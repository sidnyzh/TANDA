using FluentAssertions;
using Tanda.Domain.Rules;
using Xunit;

namespace Tanda.Domain.Tests.Rules;

/// <summary>
/// Pruebas derivadas de la tabla de fronteras del RTF3 §5.2.
/// </summary>
public sealed class LeadTimeValidatorTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(2, false)]
    [InlineData(3, false)]
    [InlineData(4, true)]
    [InlineData(5, true)]
    [InlineData(30, true)]
    public void Validar_DiasHastaLaEntrega_RetornaValidezEsperada(int diasHastaLaEntrega, bool esValidoEsperado)
    {
        var Solicitud = new DateOnly(2026, 10, 10);
        var Entrega = Solicitud.AddDays(diasHastaLaEntrega);

        var resultado = LeadTimeValidator.Validate(Solicitud, Entrega, 4);

        resultado.IsValid.Should().Be(esValidoEsperado);
    }

    [Fact]
    public void Validar_FechaDeEntregaAnteriorALaSolicitud_RetornaFalse()
    {
        var Solicitud = new DateOnly(2026, 10, 10);
        var Entrega = Solicitud.AddDays(-1);

        var resultado = LeadTimeValidator.Validate(Solicitud, Entrega, 4);

        resultado.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_CuandoRechaza_ReasonContienePrimeraFechaAdmisible()
    {
        var Solicitud = new DateOnly(2026, 10, 10);
        var Entrega = Solicitud.AddDays(3);

        var resultado = LeadTimeValidator.Validate(Solicitud, Entrega, 4);

        resultado.IsValid.Should().BeFalse();
        resultado.Reason.Should().Contain("14/10/2026");
    }

    [Fact]
    public void Validar_ConMinimumLeadDaysCero_LaEntregaMismoDiaEsValida()
    {
        var Solicitud = new DateOnly(2026, 10, 10);
        var Entrega = Solicitud;

        var resultado = LeadTimeValidator.Validate(Solicitud, Entrega, 0);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validar_ConMinimumLeadDaysNegativo_LanzaArgumentOutOfRangeException()
    {
        var Solicitud = new DateOnly(2026, 10, 10);
        var Entrega = Solicitud.AddDays(4);

        var excepcion = Record.Exception(() => LeadTimeValidator.Validate(Solicitud, Entrega, -1));

        excepcion.Should().BeOfType<ArgumentOutOfRangeException>()
            .Which.ParamName.Should().Be("minimumLeadDays");
    }
}
