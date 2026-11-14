using Microsoft.EntityFrameworkCore;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Infrastructure.Repositories;

public sealed class SizeRepository(TandaDbContext db) : ISizeRepository
{
    public async Task<IReadOnlyList<Size>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Sizes
            .AsNoTracking()
            .OrderBy(size => size.ProductId)
            .ThenBy(size => size.Name)
            .ToListAsync(cancellationToken);

    public Task<Size?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Sizes.AsNoTracking().SingleOrDefaultAsync(size => size.Id == id, cancellationToken);

    public async Task AddAsync(Size size, CancellationToken cancellationToken = default)
    {
        db.Sizes.Add(size);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Size size, CancellationToken cancellationToken = default)
    {
        db.Sizes.Update(size);
        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await db.Sizes.FindAsync([id], cancellationToken);
        if (item is not null)
        {
            db.Sizes.Remove(item);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    public Task<bool> ExistsWithNameAsync(Guid productId, string name, Guid? excludeId, CancellationToken cancellationToken = default) =>
        db.Sizes.AnyAsync(size => size.ProductId == productId && size.Name == name && (!excludeId.HasValue || size.Id != excludeId.Value), cancellationToken);
}
