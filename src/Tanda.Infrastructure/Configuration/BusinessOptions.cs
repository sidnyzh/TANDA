using System.ComponentModel.DataAnnotations;

namespace Tanda.Infrastructure.Configuration;

public sealed class BusinessOptions
{
    public const string SectionName = "Business";

    [Range(0.01, 1.0)]
    public decimal MinimumDepositRate { get; set; }

    [Range(1, 720)]
    public int QuoteValidityHours { get; set; }

    [Range(0, 365)]
    public int MinimumDaysToModify { get; set; }

    [Range(0, 365)]
    public int FullRefundDays { get; set; }

    [Range(1, 1000)]
    public int DefaultDailyCapacity { get; set; }
}
