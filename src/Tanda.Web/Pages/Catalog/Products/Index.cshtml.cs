using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;

namespace Tanda.Web.Pages.Catalog.Products;

[Authorize(Roles = "Administrator")]
public class IndexModel(IProductRepository products) : PageModel
{
    public IReadOnlyList<Product> Items { get; private set; } = [];
    public bool IncludeInactive { get; set; }
    public string? Category { get; set; }
    public SelectList Categories => new(ProductCategories.All, Category);
    public async Task OnGetAsync(bool includeInactive = false, string? category = null, CancellationToken cancellationToken = default)
    { IncludeInactive = includeInactive; Category = category; var items = await products.GetAllAsync(includeInactive, cancellationToken); Items = string.IsNullOrWhiteSpace(category) || !ProductCategories.Contains(category) ? items : items.Where(p => p.Category == category).ToList(); }
}
