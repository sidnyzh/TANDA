using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanda.Domain.Entities;

namespace Tanda.Infrastructure.Persistence.Configurations;

public sealed class OrderAdditionConfiguration : IEntityTypeConfiguration<OrderAddition>
{
    public void Configure(EntityTypeBuilder<OrderAddition> builder)
    {
        builder.HasKey(orderAddition => new { orderAddition.OrderId, orderAddition.AdditionId });

        builder.Property(orderAddition => orderAddition.Price)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(orderAddition => orderAddition.Order)
            .WithMany(order => order.OrderAdditions)
            .HasForeignKey(orderAddition => orderAddition.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(orderAddition => orderAddition.Addition)
            .WithMany()
            .HasForeignKey(orderAddition => orderAddition.AdditionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable("OrderAdditions");
    }
}