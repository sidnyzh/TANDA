using FluentAssertions;
using Tanda.Domain.Rules;

namespace Tanda.Domain.Tests.Rules;

public class LeadTimeValidatorTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, 2)]
    [InlineData(2, 5)]
    public void Validate_CumpleConElPlazo_MinimoYSuperiores_RetornaValido(int minimumLeadDays, int leadDays)
    {
        var requestDate = new DateOnly(2024, 1, 10);
        var deliveryDate = requestDate.AddDays(leadDays);

        var result = LeadTimeValidator.Validate(requestDate, deliveryDate, minimumLeadDays);

        result.IsValid.Should().BeTrue();
        result.Reason.Should().BeNull();
    }

    [Fact]
    public void Validate_NoCumpleConElPlazo_RetornaInvalidoConReasonEsperado()
    {
        var requestDate = new DateOnly(2024, 1, 10);
        var deliveryDate = new DateOnly(2024, 1, 11);

        var result = LeadTimeValidator.Validate(requestDate, deliveryDate, 3);

        result.IsValid.Should().BeFalse();
        result.Reason.Should().Be("Se requieren al menos 3 días de anticipación. La primera fecha admisible es 13/01/2024.");
    }

    [Fact]
    public void Validate_MinimumLeadDaysNegativo_LanzaArgumentOutOfRangeException()
    {
        var requestDate = new DateOnly(2024, 1, 10);
        var deliveryDate = new DateOnly(2024, 1, 15);

        Action action = () => LeadTimeValidator.Validate(requestDate, deliveryDate, -1);

        var exception = action.Should().Throw<ArgumentOutOfRangeException>().Which;
        exception.ParamName.Should().Be("minimumLeadDays");
    }
}
