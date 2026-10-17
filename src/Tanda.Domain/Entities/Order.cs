namespace Tanda.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;
}
