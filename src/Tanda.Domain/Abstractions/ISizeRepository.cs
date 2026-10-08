using Tanda.Domain.Entities;

namespace Tanda.Domain.Abstractions;

public interface ISizeRepository
{
    Task<Size?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Size size, CancellationToken cancellationToken = default);
    Task UpdateAsync(Size size, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameAsync(Guid productId, string name, Guid? excludeId, CancellationToken cancellationToken = default);
}
