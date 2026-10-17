using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tanda.Domain.Common;
using Tanda.Domain.Entities;
using Tanda.Domain.Services;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Web.Pages.Customers;

[Authorize(Roles = "Administrador,Vendedor")]
public class CreateModel(CustomerRegistrationService registrationService, TandaDbContext db) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    public SelectList DocumentTypes => new(new[]
    {
        new { Value = "CC", Text = "CC" },
        new { Value = "CE", Text = "CE" },
        new { Value = "NIT", Text = "NIT" },
        new { Value = "PA", Text = "PA" }
    }, "Value", "Text", Input.DocumentType);

    public sealed class InputModel
    {
        [Required(ErrorMessage = "El nombre completo es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre completo no puede superar 150 caracteres.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione el tipo de documento.")]
        [StringLength(10, ErrorMessage = "El tipo de documento no puede superar 10 caracteres.")]
        public string DocumentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de documento es requerido.")]
        [StringLength(20, ErrorMessage = "El número de documento no puede superar 20 caracteres.")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede superar 20 caracteres.")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "El correo no puede superar 150 caracteres.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public string? Email { get; set; }

        public bool DataConsentGranted { get; set; }
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FullName = Input.FullName,
            DocumentType = Input.DocumentType,
            DocumentNumber = Input.DocumentNumber,
            Phone = Input.Phone,
            Email = Input.Email,
            DataConsentDate = Input.DataConsentGranted ? DateOnly.FromDateTime(DateTime.Today) : null
        };

        var result = await registrationService.RegisterAsync(customer, cancellationToken);
        if (!result.IsValid)
        {
            ModelState.AddModelError(string.Empty, result.Reason ?? "No fue posible registrar el cliente.");
            return Page();
        }

        await db.SaveChangesAsync(cancellationToken);
        TempData["Message"] = "Cliente guardado correctamente.";
        return RedirectToPage("Index");
    }
}
