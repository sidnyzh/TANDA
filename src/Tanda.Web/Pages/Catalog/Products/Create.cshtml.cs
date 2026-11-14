using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;

namespace Tanda.Web.Pages.Catalog.Products;

[Authorize(Roles = "Administrator")]
public class CreateModel(IProductRepository products) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public SelectList Categories => new(ProductCategories.All, Input.Category);

    public class InputModel
    {
        [Required(ErrorMessage = "El nombre es requerido.")][StringLength(120, ErrorMessage = "El nombre no puede superar 120 caracteres.")] public string Name { get; set; } = "";
        [Required(ErrorMessage = "Seleccione una categoría")][StringLength(60, ErrorMessage = "La categoría no puede superar 60 caracteres.")] public string? Category { get; set; }
        [Required(ErrorMessage = "La anticipación mínima es requerida.")][Range(1, 60, ErrorMessage = "La anticipación debe estar entre 1 y 60 días.")] public int MinimumLeadDays { get; set; }
        public List<SizeInput> Sizes { get; set; } = new();
    }

    public class SizeInput
    {
        public bool Selected { get; set; }
        public string Name { get; set; } = "";
        public decimal BasePrice { get; set; }
        public int Esfuerzo { get; set; }
    }

    public void OnGet()
    {
        Input.Sizes = SizePresets.All.Select(name => new SizeInput { Name = name }).ToList();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ProductCategories.Contains(Input.Category)) ModelState.AddModelError("Input.Category", "La categoría seleccionada no es válida");
        if (!Input.Sizes.Any(size => size.Selected)) ModelState.AddModelError(string.Empty, "Seleccione al menos un tamaño");

        for (var i = 0; i < Input.Sizes.Count; i++)
        {
            var size = Input.Sizes[i];
            if (!size.Selected) continue;
            if (!SizePresets.Contains(size.Name)) ModelState.AddModelError($"Input.Sizes[{i}].Name", "El tamaño seleccionado no es válido.");
            if (size.BasePrice <= 0) ModelState.AddModelError($"Input.Sizes[{i}].BasePrice", "El precio debe ser mayor que cero.");
            if (size.Esfuerzo is < 1 or > 10) ModelState.AddModelError($"Input.Sizes[{i}].Esfuerzo", "El esfuerzo debe estar entre 1 y 10.");
        }

        if (!ModelState.IsValid) return Page();
        if (await products.ExistsActiveWithNameAsync(Input.Name, null, cancellationToken))
        {
            ModelState.AddModelError("Input.Name", "Ya existe un producto activo con ese nombre");
            return Page();
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = Input.Name,
            Category = Input.Category,
            MinimumLeadDays = Input.MinimumLeadDays,
            IsActive = true
        };

        foreach (var size in Input.Sizes.Where(size => size.Selected))
        {
            product.Sizes.Add(new Size
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Name = size.Name,
                BasePrice = size.BasePrice,
                Esfuerzo = size.Esfuerzo
            });
        }

        await products.AddAsync(product, cancellationToken);
        TempData["Message"] = "Producto guardado correctamente.";
        return RedirectToPage("Index");
    }
}
