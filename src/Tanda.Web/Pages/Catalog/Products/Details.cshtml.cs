using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;

namespace Tanda.Web.Pages.Catalog.Products;
[Authorize(Roles="Administrator")]
public class DetailsModel(IProductRepository products) : PageModel
{
 public Product Product { get; private set; } = null!;
 public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken ct) { Product = await products.GetByIdAsync(id,ct) ?? null!; return Product is null ? NotFound() : Page(); }
}
