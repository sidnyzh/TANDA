using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.AspNetCore.Mvc.RazorPages; using Tanda.Domain.Abstractions;
namespace Tanda.Web.Pages.Catalog.Additions;
[Authorize(Roles="Administrator")] public class DeleteModel(IAdditionRepository additions):PageModel{public async Task<IActionResult> OnPostAsync(Guid id,Guid productId,CancellationToken ct){await additions.RemoveAsync(id,ct);TempData["Message"]="Adición eliminada correctamente.";return RedirectToPage("/Catalog/Products/Details",new{id=productId});}}
