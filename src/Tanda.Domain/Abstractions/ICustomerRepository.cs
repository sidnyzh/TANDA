using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tanda.Domain.Abstractions
{
    public interface ICustomerRepository
    {
        Task<bool> ExistsByDocumentAsync(string documentType, string documentNumber, CancellationToken cancellationToken = default);

        Task<global::Tanda.Domain.Entities.Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task AddAsync(global::Tanda.Domain.Entities.Customer customer, CancellationToken cancellationToken = default);

        /// <summary>
        /// When <paramref name="term" /> is null, empty, or only whitespace, returns the full list ordered by name.
        /// When a value is provided, searches partial matches in <see cref="global::Tanda.Domain.Entities.Customer.DocumentNumber" />, <see cref="global::Tanda.Domain.Entities.Customer.FullName" />, and <see cref="global::Tanda.Domain.Entities.Customer.Phone" />.
        /// Returns an empty list, never null, when there are no matches.
        /// </summary>
        Task<IReadOnlyList<global::Tanda.Domain.Entities.Customer>> SearchAsync(
            string? term,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the customer's orders ordered by request date descending, and an empty list when the customer has no orders or does not exist.
        /// </summary>
        Task<IReadOnlyList<global::Tanda.Domain.ReadModels.CustomerOrderSummary>> GetOrderHistoryAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);
    }
}
