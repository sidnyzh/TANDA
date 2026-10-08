using Microsoft.EntityFrameworkCore;
using Tanda.Domain.Abstractions;
using Tanda.Domain.Entities;
using Tanda.Infrastructure.Persistence;

namespace Tanda.Infrastructure.Repositories;

public sealed class ProductRepository(TandaDbContext db) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken = default) =>
        await db.Products.AsNoTracking().Include(p => p.Sizes).Where(p => includeInactive || p.IsActive).OrderBy(p => p.Name).ToListAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Products.Include(p => p.Sizes).Include(p => p.Additions).SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ExistsActiveWithNameAsync(string name, Guid? excludeId, CancellationToken cancellationToken = default) =>
        db.Products.AnyAsync(p => p.IsActive && p.Name == name && (!excludeId.HasValue || p.Id != excludeId.Value), cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default) { db.Products.Add(product); await db.SaveChangesAsync(cancellationToken); }
    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default) { db.Products.Update(product); await db.SaveChangesAsync(cancellationToken); }
    public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await db.Products.FindAsync([id], cancellationToken);
        if (product is not null) { product.IsActive = false; await db.SaveChangesAsync(cancellationToken); }
    }
}
