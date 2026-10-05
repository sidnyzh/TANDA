namespace Tanda.Domain.Abstractions
{
    public interface IBusinessParameters
    {
        decimal MinimumDepositRate { get; }
        int QuoteValidityHours { get; }
        int MinimumDaysToModify { get; }
        int FullRefundDays { get; }
        int DefaultDailyCapacity { get; }
    }
}
