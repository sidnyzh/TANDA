using Microsoft.EntityFrameworkCore;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Infrastructure.Repositories;

public sealed class CustomerRepository(TandaDbContext db) : ICustomerRepository
{
    public Task<bool> ExistsByDocumentAsync(string documentType, string documentNumber, CancellationToken cancellationToken = default) =>
        db.Customers.AsNoTracking().AnyAsync(customer => customer.DocumentType == documentType && customer.DocumentNumber == documentNumber, cancellationToken);

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Customers.AsNoTracking().SingleOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        db.Customers.Add(customer);
        return Task.CompletedTask;
    }
}
