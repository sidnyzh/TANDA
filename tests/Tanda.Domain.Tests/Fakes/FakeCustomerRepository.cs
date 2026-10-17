using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;

namespace Tanda.Domain.Tests.Fakes;

public sealed class FakeCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers = new();

    public int ExistsCallCount { get; private set; }

    public int AddCallCount { get; private set; }

    public void PreloadCustomers(IEnumerable<Customer> customers)
    {
        _customers.AddRange(customers);
    }

    public Task<bool> ExistsByDocumentAsync(string documentType, string documentNumber, CancellationToken cancellationToken = default)
    {
        ExistsCallCount++;
        return Task.FromResult(_customers.Any(customer => customer.DocumentType == documentType && customer.DocumentNumber == documentNumber));
    }

    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_customers.SingleOrDefault(customer => customer.Id == id));

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        AddCallCount++;
        _customers.Add(customer);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Customer>> SearchAsync(string? term, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Customer> result;

        if (string.IsNullOrWhiteSpace(term))
        {
            result = _customers
                .OrderBy(customer => customer.FullName)
                .ToList();
        }
        else
        {
            var trimmedTerm = term.Trim();
            result = _customers
                .Where(customer =>
                    customer.DocumentNumber.Contains(trimmedTerm, StringComparison.OrdinalIgnoreCase) ||
                    customer.FullName.Contains(trimmedTerm, StringComparison.OrdinalIgnoreCase) ||
                    customer.Phone.Contains(trimmedTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(customer => customer.FullName)
                .ToList();
        }

        return Task.FromResult(result);
    }
}
