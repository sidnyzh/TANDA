using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tanda.Web.Pages.Catalog.Products;

[Authorize(Roles = "Administrator")]
public class EditModel(IProductRepository products, ISizeRepository sizes) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public SelectList Categories => new(ProductCategories.All, Input.Category);
    public HashSet<string> RegisteredSizeNames { get; private set; } = [];

    public class InputModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage="El nombre es requerido.")][StringLength(120, ErrorMessage="El nombre no puede superar 120 caracteres.")] public string Name { get; set; } = "";
        [Required(ErrorMessage="Seleccione una categor?a")][StringLength(60,ErrorMessage="La categor?a no puede superar 60 caracteres.")] public string? Category { get; set; }
        [Required(ErrorMessage="La anticipaci?n m?nima es requerida.")][Range(1,60,ErrorMessage="La anticipaci?n debe estar entre 1 y 60 d?as.")] public int MinimumLeadDays { get; set; }
        public List<SizeInput> Sizes { get; set; } = new();
    }

    public class SizeInput
    {
        public bool Selected { get; set; }
        public string Name { get; set; } = "";
        public decimal BasePrice { get; set; }
        public int Esfuerzo { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct)
    {
        var product = await products.GetByIdAsync(id, ct);
        if (product is null) return NotFound();

        Input = new()
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            MinimumLeadDays = product.MinimumLeadDays,
            Sizes = SizePresets.All.Select(name =>
            {
                var existing = product.Sizes.SingleOrDefault(size => size.Name == name);
                return new SizeInput
                {
                    Selected = existing is not null,
                    Name = name,
                    BasePrice = existing?.BasePrice ?? 0,
                    Esfuerzo = existing?.Esfuerzo ?? 0
                };
            }).ToList()
        };
        SetRegisteredSizes(product);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        var product = await products.GetByIdAsync(Input.Id, ct);
        if (product is null) return NotFound();
        SetRegisteredSizes(product);

        if (!ProductCategories.Contains(Input.Category)) ModelState.AddModelError("Input.Category", "La categor?a seleccionada no es v?lida");

        for (var i = 0; i < Input.Sizes.Count; i++)
        {
            var size = Input.Sizes[i];
            if (RegisteredSizeNames.Contains(size.Name) || !size.Selected) continue;

            if (!SizePresets.Contains(size.Name)) ModelState.AddModelError($"Input.Sizes[{i}].Name", "El tamaño seleccionado no es válido.");
            if (size.BasePrice <= 0) ModelState.AddModelError($"Input.Sizes[{i}].BasePrice", "El precio debe ser mayor que cero.");
            if (size.Esfuerzo is < 1 or > 10) ModelState.AddModelError($"Input.Sizes[{i}].Esfuerzo", "El esfuerzo debe estar entre 1 y 10.");
        }

        if (!ModelState.IsValid) return Page();
        if (await products.ExistsActiveWithNameAsync(Input.Name, Input.Id, ct)) { ModelState.AddModelError("Input.Name", "Ya existe un producto activo con ese nombre"); return Page(); }

        var newNames = new HashSet<string>();
        for (var i = 0; i < Input.Sizes.Count; i++)
        {
            var size = Input.Sizes[i];
            if (!size.Selected || RegisteredSizeNames.Contains(size.Name)) continue;
            if (!newNames.Add(size.Name) || await sizes.ExistsWithNameAsync(product.Id, size.Name, null, ct))
            {
                ModelState.AddModelError($"Input.Sizes[{i}].Name", "Ese tamaño ya está registrado.");
                continue;
            }

            product.Sizes.Add(new Size
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Name = size.Name,
                BasePrice = size.BasePrice,
                Esfuerzo = size.Esfuerzo
            });
        }

        if (!ModelState.IsValid) return Page();
        product.Name=Input.Name; product.Category=Input.Category; product.MinimumLeadDays=Input.MinimumLeadDays;
        await products.UpdateAsync(product,ct); TempData["Message"]="Producto guardado correctamente."; return RedirectToPage("Index");
    }

    private void SetRegisteredSizes(Product product) => RegisteredSizeNames = product.Sizes.Select(size => size.Name).ToHashSet();
}
