using Tanda.Domain.ReadModels;

namespace Tanda.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; set; }

    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public Guid SizeId { get; set; }

    public Size Size { get; set; } = null!;

    public string Number { get; set; } = string.Empty;

    public DateOnly RequestDate { get; set; }

    public DateOnly DeliveryDate { get; set; }

    public OrderStatus Status { get; set; }

    public int Quantity { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public ICollection<OrderAddition> OrderAdditions { get; set; } = new List<OrderAddition>();

    public int RequiredEsfuerzo() => Size.Esfuerzo * Quantity;

    public decimal PaidAmount() => Payments.Sum(p => p.Amount);

    public decimal Balance() => TotalAmount - PaidAmount();
}
