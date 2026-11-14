using FluentAssertions;
using System;
using Xunit;
using Tanda.Domain.Rules;

namespace Tanda.Domain.Tests.Rules;

/// <summary>
/// Pruebas de la calculadora de capacidad.
/// </summary>
public sealed class CapacityCalculatorTests
{
    private const int Total = 20;
    private const int Required = 5;

    [Theory]
    [InlineData(0, 20)]
    [InlineData(10, 10)]
    [InlineData(20, 0)]
    [InlineData(25, -5)]
    public void GetAvailable_Comprometido_RetornaDisponibleEsperado(int comprometido, int disponibleEsperado)
    {
        var disponible = CapacityCalculator.GetAvailable(Total, comprometido);

        disponible.Should().Be(disponibleEsperado);
    }

    [Theory]
    [InlineData(14, true)]
    [InlineData(15, true)]
    [InlineData(16, false)]
    [InlineData(20, false)]
    public void HasRoomFor_Comprometido_RetornaEsperado(int comprometido, bool esperado)
    {
        var tieneLugar = CapacityCalculator.HasRoomFor(Total, comprometido, Required);

        tieneLugar.Should().Be(esperado);
    }

    [Fact]
    public void HasRoomFor_EsfuerzoCero_FechaLlena_RetornaTrue()
    {
        var tieneLugar = CapacityCalculator.HasRoomFor(Total, Total, 0);

        tieneLugar.Should().BeTrue();
    }

    [Fact]
    public void HasRoomFor_FechaSobrecomprometida_RequeridoUno_RetornaFalse()
    {
        var tieneLugar = CapacityCalculator.HasRoomFor(Total, 25, 1);

        tieneLugar.Should().BeFalse();
    }
}