using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.AspNetCore.Mvc.RazorPages; using Tanda.Domain.Abstractions;
namespace Tanda.Web.Pages.Catalog.Sizes;
[Authorize(Roles="Administrator")] public class DeleteModel(ISizeRepository sizes):PageModel{public async Task<IActionResult> OnPostAsync(Guid id,Guid productId,CancellationToken ct){await sizes.RemoveAsync(id,ct);TempData["Message"]="Tamaño eliminado correctamente.";return RedirectToPage("/Catalog/Products/Details",new{id=productId});}}
