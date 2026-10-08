using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
namespace Tanda.Web.Pages.Catalog.Sizes;
[Authorize(Roles="Administrator")] public class CreateModel(ISizeRepository sizes, IProductRepository products) : PageModel
{
 [BindProperty] public InputModel Input {get;set;}=new(); public Product? Product {get;private set;}
 public class InputModel { public Guid ProductId{get;set;} [Required(ErrorMessage="El nombre es requerido.")][StringLength(60,ErrorMessage="El nombre no puede superar 60 caracteres.")] public string Name{get;set;}=""; [Required(ErrorMessage="El precio es requerido.")][Range(typeof(decimal),"0.01","79228162514264337593543950335",ErrorMessage="El precio debe ser mayor que cero.")] public decimal BasePrice{get;set;} [Required(ErrorMessage="El esfuerzo es requerido.")][Range(1,10,ErrorMessage="El esfuerzo debe estar entre 1 y 10.")] public int Esfuerzo{get;set;} }
 public async Task<IActionResult> OnGetAsync(Guid productId,CancellationToken ct){Product=await products.GetByIdAsync(productId,ct); if(Product is null)return NotFound();Input.ProductId=productId;return Page();}
 public async Task<IActionResult> OnPostAsync(CancellationToken ct){Product=await products.GetByIdAsync(Input.ProductId,ct);if(Product is null)return NotFound();if(!ModelState.IsValid)return Page();if(await sizes.ExistsWithNameAsync(Input.ProductId,Input.Name,null,ct)){ModelState.AddModelError("Input.Name","Ya existe un tamaño con ese nombre para este producto");return Page();}await sizes.AddAsync(new Size{Id=Guid.NewGuid(),ProductId=Input.ProductId,Name=Input.Name,BasePrice=Input.BasePrice,Esfuerzo=Input.Esfuerzo},ct);TempData["Message"]="Tamaño guardado correctamente.";return RedirectToPage("/Catalog/Products/Details",new{id=Input.ProductId});}
}
