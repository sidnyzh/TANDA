using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;

namespace Tanda.Web.Pages.Catalog.Products;
[Authorize(Roles="Administrator")]
public class DeactivateModel(IProductRepository products) : PageModel
{
 public Product Product { get; private set; } = null!;
 public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct) { Product=await products.GetByIdAsync(id,ct) ?? null!; return Product is null ? NotFound() : Page(); }
 public async Task<IActionResult> OnPostAsync(Guid id, CancellationToken ct) { await products.DeactivateAsync(id,ct); TempData["Message"]="Producto inactivado correctamente."; return RedirectToPage("Index"); }
}
