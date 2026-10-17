using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;

namespace Tanda.Web.Pages.Customers;

[Authorize(Roles = "Administrador,Vendedor")]
public class IndexModel(ICustomerRepository customers) : PageModel
{
    public IReadOnlyList<Customer> Items { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public bool HasSearchTerm => !string.IsNullOrWhiteSpace(SearchTerm);

    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        Items = await customers.SearchAsync(SearchTerm, cancellationToken);
    }
}
