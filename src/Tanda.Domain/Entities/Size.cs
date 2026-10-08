namespace Tanda.Domain.Entities;

public sealed class Size
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public string Name { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }

    public int Esfuerzo { get; set; }
}
