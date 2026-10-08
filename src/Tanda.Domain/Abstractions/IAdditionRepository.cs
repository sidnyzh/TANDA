using Tanda.Domain.Entities;

namespace Tanda.Domain.Abstractions;

public interface IAdditionRepository
{
    Task<Addition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Addition addition, CancellationToken cancellationToken = default);
    Task UpdateAsync(Addition addition, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameAsync(Guid productId, string name, Guid? excludeId, CancellationToken cancellationToken = default);
}
