using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Tanda.Web.Pages.Catalog.Products;

[Authorize(Roles = "Administrator")]
public class EditModel(IProductRepository products) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = new();
    public SelectList Categories => new(ProductCategories.All, Input.Category);
    public class InputModel { public Guid Id { get; set; } [Required(ErrorMessage="El nombre es requerido.")][StringLength(120, ErrorMessage="El nombre no puede superar 120 caracteres.")] public string Name { get; set; } = ""; [Required(ErrorMessage="Seleccione una categoría")][StringLength(60,ErrorMessage="La categoría no puede superar 60 caracteres.")] public string? Category { get; set; } [Required(ErrorMessage="La anticipación mínima es requerida.")][Range(1,60,ErrorMessage="La anticipación debe estar entre 1 y 60 días.")] public int MinimumLeadDays { get; set; } }
    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct) { var p = await products.GetByIdAsync(id, ct); if (p is null) return NotFound(); Input = new() { Id=p.Id, Name=p.Name, Category=p.Category, MinimumLeadDays=p.MinimumLeadDays }; return Page(); }
    public async Task<IActionResult> OnPostAsync(CancellationToken ct) { if (!ProductCategories.Contains(Input.Category)) ModelState.AddModelError("Input.Category", "La categoría seleccionada no es válida"); if (!ModelState.IsValid) return Page(); var p = await products.GetByIdAsync(Input.Id, ct); if (p is null) return NotFound(); if (await products.ExistsActiveWithNameAsync(Input.Name, Input.Id, ct)) { ModelState.AddModelError("Input.Name", "Ya existe un producto activo con ese nombre"); return Page(); } p.Name=Input.Name; p.Category=Input.Category; p.MinimumLeadDays=Input.MinimumLeadDays; await products.UpdateAsync(p,ct); TempData["Message"]="Producto guardado correctamente."; return RedirectToPage("Index"); }
}
