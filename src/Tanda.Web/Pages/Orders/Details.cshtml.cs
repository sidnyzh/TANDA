using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;

namespace Tanda.Web.Pages.Orders;

[Authorize(Roles = "Administrator,Salesperson")]
public class DetailsModel(IOrderRepository orders) : PageModel
{
    public static readonly CultureInfo CopCulture = new("es-CO");

    public Order? Order { get; private set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? CapacityWarning { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Order = await orders.GetByIdAsync(id, cancellationToken);
        if (Order is null)
        {
            return NotFound();
        }

        return Page();
    }
}