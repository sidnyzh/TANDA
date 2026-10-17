namespace Tanda.Domain.ReadModels;

public sealed record CustomerOrderSummary(
    Guid OrderId,
    string Number,
    DateOnly RequestDate,
    DateOnly DeliveryDate,
    OrderStatus Status,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal Balance);
