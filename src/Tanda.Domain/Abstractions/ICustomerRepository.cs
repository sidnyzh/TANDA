using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tanda.Domain.Abstractions
{
    public interface ICustomerRepository
    {
        Task<bool> ExistsByDocumentAsync(string documentType, string documentNumber, CancellationToken cancellationToken = default);

        Task<global::Tanda.Domain.Entities.Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(global::Tanda.Domain.Entities.Customer customer, CancellationToken cancellationToken = default);
    }
}
