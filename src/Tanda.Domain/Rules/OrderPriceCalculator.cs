using Tanda.Domain.Entities;

namespace Tanda.Domain.Rules;

/// <summary>
/// Calcula el total del pedido usando montos en <see cref="decimal"/>, porque <see cref="double"/> y <see cref="float"/>
/// representan valores en base 2 y no almacenan 0,1 de forma exacta, por lo que un acumulador en punto flotante
/// podría dejar el total una fracción por debajo del valor real.
/// </summary>
public static class OrderPriceCalculator
{
    public static decimal Calculate(decimal basePrice, IReadOnlyList<Addition> additions, int quantity)
    {
        ArgumentNullException.ThrowIfNull(additions);

        if (basePrice < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(basePrice));
        }

        if (quantity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        decimal additionsTotal = 0m;
        for (var i = 0; i <= additions.Count - 1; i++)
        {
            additionsTotal += additions[i].Price;
        }

        return (basePrice + additionsTotal) * quantity;
    }
}
