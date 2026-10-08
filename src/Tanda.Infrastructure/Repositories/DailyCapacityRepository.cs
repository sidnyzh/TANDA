using Microsoft.EntityFrameworkCore;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Infrastructure.Repositories;

public sealed class DailyCapacityRepository(TandaDbContext db) : IDailyCapacityRepository
{
    public Task<DailyCapacity?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default) => db.DailyCapacities.AsNoTracking().SingleOrDefaultAsync(c => c.Date == date, cancellationToken);
    public async Task<IReadOnlyList<DailyCapacity>> GetRangeAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default) => await db.DailyCapacities.AsNoTracking().Where(c => c.Date >= from && c.Date <= to).OrderBy(c => c.Date).ToListAsync(cancellationToken);
    public async Task UpsertAsync(DateOnly date, int totalEsfuerzo, CancellationToken cancellationToken = default)
    {
        var item = await db.DailyCapacities.SingleOrDefaultAsync(c => c.Date == date, cancellationToken);
        if (item is null) db.DailyCapacities.Add(new DailyCapacity { Date = date, TotalEsfuerzo = totalEsfuerzo });
        else item.TotalEsfuerzo = totalEsfuerzo;
        await db.SaveChangesAsync(cancellationToken);
    }
}
