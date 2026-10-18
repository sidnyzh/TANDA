using FluentAssertions;
using Tanda.Domain.Entities;
using Tanda.Domain.Rules;

namespace Tanda.Domain.Tests.Rules;

public class OrderPriceCalculatorTests
{
    [Fact]
    public void Calculate_AdditionsNull_LanzaArgumentNullException()
    {
        IReadOnlyList<Addition> additions = null!;

        Action action = () => OrderPriceCalculator.Calculate(10m, additions, 1);

        var exception = action.Should().Throw<ArgumentNullException>().Which;
        exception.ParamName.Should().Be("additions");
    }

    [Fact]
    public void Calculate_BasePriceNegativo_LanzaArgumentOutOfRangeException()
    {
        var additions = new List<Addition>();

        Action action = () => OrderPriceCalculator.Calculate(-0.01m, additions, 1);

        var exception = action.Should().Throw<ArgumentOutOfRangeException>().Which;
        exception.ParamName.Should().Be("basePrice");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Calculate_QuantityMenorQueUno_LanzaArgumentOutOfRangeException(int quantity)
    {
        var additions = new List<Addition>();

        Action action = () => OrderPriceCalculator.Calculate(10m, additions, quantity);

        var exception = action.Should().Throw<ArgumentOutOfRangeException>().Which;
        exception.ParamName.Should().Be("quantity");
    }

    [Fact]
    public void Calculate_AdditionsVacia_RetornaBasePorCantidad()
    {
        var additions = new List<Addition>();

        var result = OrderPriceCalculator.Calculate(12.5m, additions, 3);

        result.Should().Be(37.5m);
    }

    [Fact]
    public void Calculate_AdditionsConValores_RetornaTotalEsperado()
    {
        var additions = new List<Addition>
        {
            new() { Price = 1.5m },
            new() { Price = 2.25m }
        };

        var result = OrderPriceCalculator.Calculate(10m, additions, 2);

        result.Should().Be(27.5m);
    }
}
