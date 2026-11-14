using Tanda.Domain.Entities;

namespace Tanda.Domain.Abstractions;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task AddAsync(Order order, CancellationToken ct = default);

    Task<int> CountByYearAsync(int year, CancellationToken ct = default);
}