using FluentAssertions;
using Tanda.Domain.Rules;

namespace Tanda.Domain.Tests.Rules;

public class CustomerDocumentValidatorTests
{
    [Theory]
    [InlineData("CC", "12345")]
    [InlineData("CE", "12345678901234567890")]
    [InlineData("NIT", "00001")]
    [InlineData("PA", "987654321")]
    public void Validate_TipoValidoYNumeroValido_RetornaValido(string? documentType, string documentNumber)
    {
        var result = CustomerDocumentValidator.Validate(documentType!, documentNumber);

        result.IsValid.Should().BeTrue();
        result.Reason.Should().BeNull();
    }

    [Theory]
    [InlineData("XX", "12345")]
    [InlineData("", "12345")]
    [InlineData(null, "12345")]
    public void Validate_TipoInvalido_RetornaInvalido(string? documentType, string documentNumber)
    {
        var result = CustomerDocumentValidator.Validate(documentType!, documentNumber);

        result.IsValid.Should().BeFalse();
        result.Reason.Should().Contain("CC").And.Contain("CE").And.Contain("NIT").And.Contain("PA");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_NumeroNuloVacioOConEspacios_RetornaInvalido(string? documentNumber)
    {
        var result = CustomerDocumentValidator.Validate("CC", documentNumber!);

        result.IsValid.Should().BeFalse();
        result.Reason.Should().Be("El número de documento no puede ser nulo, vacío ni contener solo espacios.");
    }

    [Fact]
    public void Validate_NumeroConLetras_RetornaInvalido()
    {
        var result = CustomerDocumentValidator.Validate("CC", "12AB5");

        result.IsValid.Should().BeFalse();
        result.Reason.Should().Be("El número de documento debe contener únicamente dígitos.");
    }

    [Theory]
    [InlineData("1234", false)]
    [InlineData("12345", true)]
    [InlineData("12345678901234567890", true)]
    [InlineData("123456789012345678901", false)]
    public void Validate_LongitudDelNumero_RetornaResultadoEsperado(string documentNumber, bool expectedIsValid)
    {
        var result = CustomerDocumentValidator.Validate("CC", documentNumber);

        result.IsValid.Should().Be(expectedIsValid);
    }

    [Theory]
    [InlineData("CC", "1234", "El número de documento debe tener entre 5 y 20 caracteres.")]
    [InlineData("XX", "12AB5", "El tipo de documento debe ser uno de los valores admitidos: CC, CE, NIT o PA.")]
    [InlineData("CC", "12AB5", "El número de documento debe contener únicamente dígitos.")]
    [InlineData("CC", "   ", "El número de documento no puede ser nulo, vacío ni contener solo espacios.")]
    public void Validate_CondicionQueFalla_ReasonEspecifico(string? documentType, string? documentNumber, string expectedReason)
    {
        var result = CustomerDocumentValidator.Validate(documentType!, documentNumber!);

        result.IsValid.Should().BeFalse();
        result.Reason.Should().Be(expectedReason);
    }
}
