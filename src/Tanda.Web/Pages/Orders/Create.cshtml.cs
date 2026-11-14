using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Domain.Rules;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Web.Pages.Orders;

[Authorize(Roles = "Administrator,Salesperson")]
public class CreateModel(
    ICustomerRepository customers,
    IProductRepository products,
    ISizeRepository sizes,
    IAdditionRepository additions,
    IDailyCapacityRepository capacities,
    IOrderRepository orders,
    IBusinessParameters businessParameters,
    TandaDbContext db) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<CustomerOption> Customers { get; private set; } = [];

    public IReadOnlyList<ProductOption> Products { get; private set; } = [];

    public IReadOnlyList<SizeOption> Sizes { get; private set; } = [];

    public IReadOnlyList<AdditionOption> Additions { get; private set; } = [];

    public sealed class InputModel
    {
        [Required(ErrorMessage = "Seleccione un cliente.")]
        public Guid? CustomerId { get; set; }

        [Required(ErrorMessage = "Seleccione un producto.")]
        public Guid? ProductId { get; set; }

        [Required(ErrorMessage = "Seleccione un tamaño.")]
        public Guid? SizeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 1.")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Ingrese la fecha de entrega.")]
        public DateOnly DeliveryDate { get; set; }

        public List<Guid> SelectedAdditionIds { get; set; } = [];
    }

    public sealed record CustomerOption(Guid Id, string DisplayName);

    public sealed record ProductOption(Guid Id, string Name);

    public sealed record SizeOption(Guid Id, Guid ProductId, string Name, decimal BasePrice);

    public sealed record AdditionOption(Guid Id, Guid ProductId, string Name, decimal Price);

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadLookupsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        // 1. Valida el modelo
        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(cancellationToken);
            return Page();
        }

        // 2. Obtiene tamaño y adiciones seleccionadas
        var size = await sizes.GetByIdAsync(Input.SizeId!.Value, cancellationToken);
        var product = await products.GetByIdAsync(Input.ProductId!.Value, cancellationToken);

        if (size is null)
        {
            ModelState.AddModelError("Input.SizeId", "El tamaño seleccionado no existe.");
        }

        if (product is null || !product.IsActive)
        {
            ModelState.AddModelError("Input.ProductId", "El producto seleccionado no existe o está inactivo.");
        }

        var selectedAdditions = new List<Addition>();
        foreach (var additionId in Input.SelectedAdditionIds.Distinct())
        {
            var addition = await additions.GetByIdAsync(additionId, cancellationToken);
            if (addition is null || !addition.IsActive)
            {
                ModelState.AddModelError("Input.SelectedAdditionIds", "Una de las adiciones seleccionadas no es válida.");
                continue;
            }

            selectedAdditions.Add(addition);
        }

        if (!ModelState.IsValid)
        {
            await LoadLookupsAsync(cancellationToken);
            return Page();
        }

        // 3. Calcula total
        var total = OrderPriceCalculator.Calculate(size!.BasePrice, selectedAdditions, Input.Quantity);

        // 4. Valida lead time
        var today = DateOnly.FromDateTime(DateTime.Today);
        var leadTimeResult = LeadTimeValidator.Validate(today, Input.DeliveryDate, product!.MinimumLeadDays);
        if (!leadTimeResult.IsValid)
        {
            ModelState.AddModelError("Input.DeliveryDate", leadTimeResult.Reason ?? "La fecha no cumple el tiempo mínimo de anticipación.");
            await LoadLookupsAsync(cancellationToken);
            return Page();
        }

        // 5. Consulta capacidad y advierte sin interrumpir
        var capacity = await capacities.GetByDateAsync(Input.DeliveryDate, cancellationToken);
        var totalEsfuerzo = capacity?.TotalEsfuerzo ?? businessParameters.DefaultDailyCapacity;
        var committedEsfuerzo = capacity?.CommittedEsfuerzo ?? 0;
        var requiredEsfuerzo = size.Esfuerzo * Input.Quantity;

        if (!CapacityCalculator.HasRoomFor(totalEsfuerzo, committedEsfuerzo, requiredEsfuerzo))
        {
            var faltante = requiredEsfuerzo - CapacityCalculator.GetAvailable(totalEsfuerzo, committedEsfuerzo);
            TempData["CapacityWarning"] =
                $"Advertencia de capacidad: para la fecha {Input.DeliveryDate:dd/MM/yyyy} faltan {faltante} unidades de esfuerzo.";
        }

        // 6. Genera número PED-{año}-{consecutivo}
        var year = today.Year;
        var countInYear = await orders.CountByYearAsync(year, cancellationToken);
        var nextConsecutive = countInYear + 1;
        var orderNumber = $"PED-{year}-{nextConsecutive:D4}";

        // 7. Crea el pedido
        var orderId = Guid.NewGuid();

        var order = new Order
        {
            Id = orderId,
            CustomerId = Input.CustomerId.Value,
            ProductId = Input.ProductId.Value,
            SizeId = Input.SizeId.Value,
            Number = orderNumber,
            RequestDate = today,
            DeliveryDate = Input.DeliveryDate,
            Status = OrderStatus.Quoted,
            Quantity = Input.Quantity,
            TotalAmount = total,
            CreatedAt = DateTimeOffset.Now,
            OrderAdditions = selectedAdditions
                .Select(addition => new OrderAddition
                {
                    OrderId = orderId,
                    AdditionId = addition.Id,
                    Price = addition.Price
                })
                .ToList()
        };

        // 8. Guarda y redirige
        await orders.AddAsync(order, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        TempData["SuccessMessage"] = "Cotización registrada";
        return RedirectToPage("Details", new { id = order.Id });
    }

    private async Task LoadLookupsAsync(CancellationToken cancellationToken)
    {
        var customerItems = await customers.SearchAsync(null, cancellationToken);
        var productItems = await products.GetAllAsync(includeInactive: false, cancellationToken);
        var sizeItems = await sizes.GetAllAsync(cancellationToken);
        var additionItems = await additions.GetActiveAsync(cancellationToken);

        Customers = customerItems
            .Select(customer => new CustomerOption(customer.Id, $"{customer.FullName} ({customer.DocumentType} {customer.DocumentNumber})"))
            .ToList();

        Products = productItems
            .Select(product => new ProductOption(product.Id, product.Name))
            .ToList();

        Sizes = sizeItems
            .Select(size => new SizeOption(size.Id, size.ProductId, size.Name, size.BasePrice))
            .ToList();

        Additions = additionItems
            .Select(addition => new AdditionOption(addition.Id, addition.ProductId, addition.Name, addition.Price))
            .ToList();
    }
}