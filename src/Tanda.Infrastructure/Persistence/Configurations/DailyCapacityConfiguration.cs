using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tanda.Domain.Entities;

namespace Tanda.Infrastructure.Persistence.Configurations;

public sealed class DailyCapacityConfiguration : IEntityTypeConfiguration<DailyCapacity>
{
    public void Configure(EntityTypeBuilder<DailyCapacity> builder)
    {
        builder.HasKey(capacity => capacity.Date);

        builder.ToTable("DailyCapacities", table =>
        {
            table.HasCheckConstraint(
                "CK_DailyCapacities_TotalEsfuerzo",
                "[TotalEsfuerzo] > 0");
            table.HasCheckConstraint(
                "CK_DailyCapacities_CommittedEsfuerzo",
                "[CommittedEsfuerzo] >= 0");
            table.HasCheckConstraint(
                "CK_DailyCapacities_CommittedNotGreaterThanTotal",
                "[CommittedEsfuerzo] <= [TotalEsfuerzo]");
        });
    }
}
