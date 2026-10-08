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
    }
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ProductCategories.Contains(Input.Category)) ModelState.AddModelError("Input.Category", "La categoría seleccionada no es válida");
        if (!ModelState.IsValid) return Page();
        if (await products.ExistsActiveWithNameAsync(Input.Name, null, cancellationToken)) { ModelState.AddModelError("Input.Name", "Ya existe un producto activo con ese nombre"); return Page(); }
        await products.AddAsync(new Product { Id = Guid.NewGuid(), Name = Input.Name, Category = Input.Category, MinimumLeadDays = Input.MinimumLeadDays, IsActive = true }, cancellationToken);
        TempData["Message"] = "Producto guardado correctamente."; return RedirectToPage("Index");
    }
}
