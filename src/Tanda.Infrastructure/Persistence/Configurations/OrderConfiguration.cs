using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanda.Domain.Entities;

namespace Tanda.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(order => order.Id);

        builder.Property(order => order.Number)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(order => order.Number)
            .IsUnique();

        builder.Property(order => order.TotalAmount)
            .HasColumnType("decimal(18,2)");

        builder.Property(order => order.RequestDate)
            .HasColumnType("date");

        builder.Property(order => order.DeliveryDate)
            .HasColumnType("date");

        builder.Property(order => order.CreatedAt)
            .HasColumnType("datetimeoffset");

        builder.Property(order => order.Quantity)
            .IsRequired();

        builder.HasOne(order => order.Customer)
            .WithMany(customer => customer.Orders)
            .HasForeignKey(order => order.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(order => order.Product)
            .WithMany()
            .HasForeignKey(order => order.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(order => order.Size)
            .WithMany()
            .HasForeignKey(order => order.SizeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("Orders");
    }
}