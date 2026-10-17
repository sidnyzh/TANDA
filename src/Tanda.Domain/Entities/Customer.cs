namespace Tanda.Domain.Entities;

public sealed class Customer
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string DocumentType { get; set; } = string.Empty;

    public string DocumentNumber { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string? Email { get; set; }

    /// <summary>
    /// Valor nulo indica que el titular no ha autorizado el tratamiento de sus datos; una fecha registra el momento en que lo autorizó.
    /// </summary>
    public DateOnly? DataConsentDate { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}