using Microsoft.EntityFrameworkCore;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Infrastructure.Repositories;

public sealed class AdditionRepository(TandaDbContext db) : IAdditionRepository
{
    public async Task<IReadOnlyList<Addition>> GetActiveAsync(CancellationToken cancellationToken = default) =>
        await db.Additions
            .AsNoTracking()
            .Where(addition => addition.IsActive)
            .OrderBy(addition => addition.ProductId)
            .ThenBy(addition => addition.Name)
            .ToListAsync(cancellationToken);

    public Task<Addition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Additions.AsNoTracking().SingleOrDefaultAsync(addition => addition.Id == id, cancellationToken);

    public async Task AddAsync(Addition addition, CancellationToken cancellationToken = default)
    {
        db.Additions.Add(addition);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Addition addition, CancellationToken cancellationToken = default)
    {
        db.Additions.Update(addition);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await db.Additions.FindAsync([id], cancellationToken);
        if (item is not null)
        {
            db.Additions.Remove(item);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    public Task<bool> ExistsWithNameAsync(Guid productId, string name, Guid? excludeId, CancellationToken cancellationToken = default) =>
        db.Additions.AnyAsync(addition => addition.ProductId == productId && addition.Name == name && (!excludeId.HasValue || addition.Id != excludeId.Value), cancellationToken);
}
