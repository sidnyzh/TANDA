using Tanda.Domain.Entities;

namespace Tanda.Domain.Abstractions;

public interface IDailyCapacityRepository
{
    Task<DailyCapacity?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DailyCapacity>> GetRangeAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
    Task UpsertAsync(DateOnly date, int totalEsfuerzo, CancellationToken cancellationToken = default);
}
