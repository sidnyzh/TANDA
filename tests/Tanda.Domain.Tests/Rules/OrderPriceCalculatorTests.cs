using System;
using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using Tanda.Domain.Entities;
using Tanda.Domain.Rules;
using Xunit;

namespace Tanda.Domain.Tests.Rules;

/// <summary>
/// Pruebas del cálculo de precio del pedido.
/// </summary>
public sealed class OrderPriceCalculatorTests
{
    private static IReadOnlyList<Addition> BuildAdditions(params decimal[] prices)
    {
        var list = new List<Addition>(prices.Length);
        for (var i = 0; i < prices.Length; i++)
        {
            list.Add(new Addition
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                Name = $"A{i + 1}",
                Price = prices[i],
                IsActive = true
            });
        }

        return list;
    }

    [Fact]
    public void Calcular_SinAdiciones_CantidadUno_RetornaPrecioBase()
    {
        var basePrice = 150000m;
        var additions = BuildAdditions();
        var total = OrderPriceCalculator.Calculate(basePrice, additions, 1);

        total.Should().Be(150000m);
    }

    [Fact]
    public void Calcular_UnaAdicion_CantidadUno_SumaAdicionAlPrecioBase()
    {
        var basePrice = 150000m;
        var additions = BuildAdditions(15000m);
        var total = OrderPriceCalculator.Calculate(basePrice, additions, 1);

        total.Should().Be(165000m);
    }

    [Fact]
    public void Calcular_TresAdiciones_CantidadUno_SumaTodasLasAdiciones()
    {
        var basePrice = 150000m;
        var additions = BuildAdditions(15000m, 8000m, 12000m);
        var total = OrderPriceCalculator.Calculate(basePrice, additions, 1);

        total.Should().Be(185000m);
    }

    [Theory]
    [InlineData(1, "185000")]
    [InlineData(2, "370000")]
    [InlineData(5, "925000")]
    public void Calcular_TresAdiciones_VariasCantidades_RetornaTotalEsperado(int quantity, string totalEsperado)
    {
        var basePrice = 150000m;
        var additions = BuildAdditions(15000m, 8000m, 12000m);
        var expected = decimal.Parse(totalEsperado, NumberStyles.Integer, CultureInfo.InvariantCulture);

        var total = OrderPriceCalculator.Calculate(basePrice, additions, quantity);

        total.Should().Be(expected);
    }

    [Fact]
    public void Calcular_PrecisionDecimal_SumaExacta()
    {
        var basePrice = 0m;
        var additions = BuildAdditions(0.1m, 0.2m);

        var total = OrderPriceCalculator.Calculate(basePrice, additions, 1);

        total.Should().Be(0.3m);
    }

    [Fact]
    public void Calcular_AdditionsNull_ArgumentNullException()
    {
        IReadOnlyList<Addition>? additions = null;

        Action act = () => OrderPriceCalculator.Calculate(150000m, additions!, 1);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("additions");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Calcular_CantidadMenorQueUno_ArgumentOutOfRangeException(int quantity)
    {
        var basePrice = 150000m;
        var additions = BuildAdditions();

        Action act = () => OrderPriceCalculator.Calculate(basePrice, additions, quantity);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .Where(e => e.ParamName == "quantity");
    }

    [Fact]
    public void Calcular_BasePriceNegativo_ArgumentOutOfRangeException()
    {
        var basePrice = -1m;
        var additions = BuildAdditions();

        Action act = () => OrderPriceCalculator.Calculate(basePrice, additions, 1);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .Where(e => e.ParamName == "basePrice");
    }
}
