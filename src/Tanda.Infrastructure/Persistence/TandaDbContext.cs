using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tanda.Domain.Entities;

namespace Tanda.Infrastructure.Persistence;

public sealed class TandaDbContext : IdentityDbContext
{
    public TandaDbContext(DbContextOptions<TandaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Size> Sizes => Set<Size>();

    public DbSet<Addition> Additions => Set<Addition>();

    public DbSet<DailyCapacity> DailyCapacities => Set<DailyCapacity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(TandaDbContext).Assembly);
    }
}