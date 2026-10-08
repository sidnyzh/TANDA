using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanda.Domain.Entities;

namespace Tanda.Infrastructure.Persistence.Configurations;

public sealed class SizeConfiguration : IEntityTypeConfiguration<Size>
{
    public void Configure(EntityTypeBuilder<Size> builder)
    {
        builder.HasKey(size => size.Id);

        builder.Property(size => size.Name)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(size => size.BasePrice)
            .HasPrecision(18, 2);

        builder.HasIndex(size => new { size.ProductId, size.Name })
            .IsUnique();

        builder.HasOne(size => size.Product)
            .WithMany(product => product.Sizes)
            .HasForeignKey(size => size.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Sizes", table =>
        {
            table.HasCheckConstraint(
                "CK_Sizes_Esfuerzo",
                "[Esfuerzo] BETWEEN 1 AND 10");
        });
    }
}
