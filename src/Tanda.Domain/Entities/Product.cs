namespace Tanda.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Category { get; set; }

    public int MinimumLeadDays { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Size> Sizes { get; set; } = new List<Size>();

    public ICollection<Addition> Additions { get; set; } = new List<Addition>();
}
