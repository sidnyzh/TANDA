using Microsoft.EntityFrameworkCore;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Domain.ReadModels;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Infrastructure.Repositories;

public sealed class CustomerRepository(TandaDbContext db) : ICustomerRepository
{
    public Task<bool> ExistsByDocumentAsync(string documentType, string documentNumber, CancellationToken cancellationToken = default) =>
        db.Customers.AsNoTracking().AnyAsync(customer => customer.DocumentType == documentType && customer.DocumentNumber == documentNumber, cancellationToken);

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Customers.AsNoTracking().SingleOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public async Task<IReadOnlyList<CustomerOrderSummary>> GetOrderHistoryAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orders = await db.Orders
            .AsNoTracking()
            .Where(order => order.CustomerId == customerId)
            .OrderByDescending(order => order.RequestDate)
            .Select(order => new CustomerOrderSummary(
                order.Id,
                order.Number,
                order.RequestDate,
                order.DeliveryDate,
                order.Status,
                order.TotalAmount,
                order.Payments.Sum(payment => payment.Amount),
                order.TotalAmount - order.Payments.Sum(payment => payment.Amount)))
            .ToListAsync(cancellationToken);

        return orders;
    }

    public async Task<IReadOnlyList<Customer>> SearchAsync(string? term, CancellationToken cancellationToken = default)
    {
        IQueryable<Customer> query = db.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(term))
        {
            var trimmedTerm = term.Trim();
            var pattern = $"%{EscapeLike(trimmedTerm)}%";

            query = query.Where(customer =>
                EF.Functions.Like(customer.DocumentNumber, pattern, "\\") ||
                EF.Functions.Like(customer.FullName, pattern, "\\") ||
                EF.Functions.Like(customer.Phone, pattern, "\\"));
        }

        return await query
            .OrderBy(customer => customer.FullName)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        db.Customers.Add(customer);
        return Task.CompletedTask;
    }

    private static string EscapeLike(string value)
    {
        var result = value.Replace("\\", "\\\\");
        result = result.Replace("%", "\\%");
        result = result.Replace("_", "\\_");
        return result;
    }
}
