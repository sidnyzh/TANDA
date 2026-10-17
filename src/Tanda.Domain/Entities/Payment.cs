namespace Tanda.Domain.Entities;

public sealed class Payment
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public decimal Amount { get; set; }
}
