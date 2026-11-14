using Microsoft.EntityFrameworkCore;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Infrastructure.Repositories;

public sealed class OrderRepository(TandaDbContext db) : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.Product)
            .Include(order => order.Size)
            .Include(order => order.OrderAdditions)
                .ThenInclude(orderAddition => orderAddition.Addition)
            .Include(order => order.Payments)
            .SingleOrDefaultAsync(order => order.Id == id, ct);

    public Task AddAsync(Order order, CancellationToken ct = default)
    {
        db.Orders.Add(order);
        return Task.CompletedTask;
    }

    public Task<int> CountByYearAsync(int year, CancellationToken ct = default) =>
        db.Orders
            .AsNoTracking()
            .CountAsync(order => order.RequestDate.Year == year, ct);
}