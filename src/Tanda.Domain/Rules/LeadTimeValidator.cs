using System;
using System.Globalization;
using Tanda.Domain.Common;

namespace Tanda.Domain.Rules;

/// <summary>
/// Valida el plazo mínimo usando aritmética entera sobre <see cref="DateOnly.DayNumber"/>,
/// para que la comparación en el límite no dependa del redondeo de punto flotante.
/// </summary>
public static class LeadTimeValidator
{
    public static ValidationResult Validate(DateOnly requestDate, DateOnly deliveryDate, int minimumLeadDays)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(minimumLeadDays);

        var leadDays = deliveryDate.DayNumber - requestDate.DayNumber;
        if (leadDays >= minimumLeadDays)
        {
            return ValidationResult.Valid();
        }

        var firstAdmissibleDate = requestDate.AddDays(minimumLeadDays)
            .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        return ValidationResult.Invalid(
            $"Se requieren al menos {minimumLeadDays} días de anticipación. La primera fecha admisible es {firstAdmissibleDate}.");
    }
}
