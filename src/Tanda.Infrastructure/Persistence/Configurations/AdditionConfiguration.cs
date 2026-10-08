using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanda.Domain.Entities;

namespace Tanda.Infrastructure.Persistence.Configurations;

public sealed class AdditionConfiguration : IEntityTypeConfiguration<Addition>
{
    public void Configure(EntityTypeBuilder<Addition> builder)
    {
        builder.HasKey(addition => addition.Id);

        builder.Property(addition => addition.Name)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(addition => addition.Price)
            .HasPrecision(18, 2);

        builder.HasIndex(addition => new { addition.ProductId, addition.Name })
            .IsUnique();

        builder.HasOne(addition => addition.Product)
            .WithMany(product => product.Additions)
            .HasForeignKey(addition => addition.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
