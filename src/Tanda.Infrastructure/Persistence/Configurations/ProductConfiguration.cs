using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanda.Domain.Entities;

namespace Tanda.Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(product => product.Id);

        builder.Property(product => product.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(product => product.Category)
            .HasMaxLength(60);

        builder.HasIndex(product => product.Name)
            .IsUnique()
            .HasFilter("[IsActive] = 1");

        builder.ToTable("Products", table =>
        {
            table.HasCheckConstraint(
                "CK_Products_MinimumLeadDays",
                "[MinimumLeadDays] BETWEEN 1 AND 60");
        });
    }
}
