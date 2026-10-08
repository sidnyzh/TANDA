using Microsoft.EntityFrameworkCore;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Infrastructure.Repositories;

public sealed class AdditionRepository(TandaDbContext db) : IAdditionRepository
{
    public Task<Addition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => db.Additions.AsNoTracking().SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
    public async Task AddAsync(Addition addition, CancellationToken cancellationToken = default) { db.Additions.Add(addition); await db.SaveChangesAsync(cancellationToken); }
    public async Task UpdateAsync(Addition addition, CancellationToken cancellationToken = default) { db.Additions.Update(addition); await db.SaveChangesAsync(cancellationToken); }
    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken = default) { var item = await db.Additions.FindAsync([id], cancellationToken); if (item is not null) { db.Additions.Remove(item); await db.SaveChangesAsync(cancellationToken); } }
    public Task<bool> ExistsWithNameAsync(Guid productId, string name, Guid? excludeId, CancellationToken cancellationToken = default) => db.Additions.AnyAsync(a => a.ProductId == productId && a.Name == name && (!excludeId.HasValue || a.Id != excludeId.Value), cancellationToken);
}
