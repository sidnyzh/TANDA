using Tanda.Domain.ReadModels;

namespace Tanda.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public string Number { get; set; } = string.Empty;

    public DateOnly RequestDate { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public OrderStatus Status { get; set; }

    public decimal TotalAmount { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
