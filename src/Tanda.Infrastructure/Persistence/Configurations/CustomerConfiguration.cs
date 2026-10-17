using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanda.Domain.Entities;

namespace Tanda.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(customer => customer.DocumentType)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(customer => customer.DocumentNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(customer => customer.Phone)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(customer => customer.Email)
            .HasMaxLength(150);

        builder.Property(customer => customer.DataConsentDate)
            .HasColumnType("date");

        builder.HasIndex(customer => new { customer.DocumentType, customer.DocumentNumber })
            .IsUnique();

        builder.ToTable("Customers", table =>
        {
            table.HasCheckConstraint(
                "CK_Customers_DocumentType",
                "[DocumentType] IN ('CC', 'CE', 'NIT', 'PA')");
        });
    }
}
