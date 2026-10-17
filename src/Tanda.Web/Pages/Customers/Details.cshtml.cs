using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Domain.ReadModels;

namespace Tanda.Web.Pages.Customers;

[Authorize(Roles = "Administrador,Vendedor")]
public class DetailsModel(ICustomerRepository customers) : PageModel
{
    public Customer? Customer { get; private set; }

    public IReadOnlyList<CustomerOrderSummary> Orders { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Customer = await customers.GetByIdAsync(id, cancellationToken);
        if (Customer is null)
        {
            return NotFound();
        }

        Orders = await customers.GetOrderHistoryAsync(id, cancellationToken);
        return Page();
    }
}
