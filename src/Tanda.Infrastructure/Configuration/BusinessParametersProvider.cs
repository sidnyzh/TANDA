using Microsoft.Extensions.Options;
using Tanda.Domain.Abstractions;

namespace Tanda.Infrastructure.Configuration;

internal sealed class BusinessParametersProvider : IBusinessParameters
{
    private readonly IOptionsMonitor<BusinessOptions> _options;

    public BusinessParametersProvider(IOptionsMonitor<BusinessOptions> options)
    {
        _options = options;
    }

    private BusinessOptions Current => _options.CurrentValue;

    public decimal MinimumDepositRate => Current.MinimumDepositRate;

    public int QuoteValidityHours => Current.QuoteValidityHours;

    public int MinimumDaysToModify => Current.MinimumDaysToModify;

    public int FullRefundDays => Current.FullRefundDays;

    public int DefaultDailyCapacity => Current.DefaultDailyCapacity;
}