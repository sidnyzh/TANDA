using Tanda.Domain.Common;

namespace Tanda.Domain.Rules;

public static class CustomerDocumentValidator
{
    public static ValidationResult Validate(string documentType, string documentNumber)
    {
        if (documentType != "CC" && documentType != "CE" && documentType != "NIT" && documentType != "PA")
        {
            return ValidationResult.Invalid("El tipo de documento debe ser uno de los valores admitidos: CC, CE, NIT o PA.");
        }

        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            return ValidationResult.Invalid("El número de documento no puede ser nulo, vacío ni contener solo espacios.");
        }

        for (var i = 0; i < documentNumber.Length; i++)
        {
            if (!char.IsDigit(documentNumber[i]))
            {
                return ValidationResult.Invalid("El número de documento debe contener únicamente dígitos.");
            }
        }

        if (documentNumber.Length < 5 || documentNumber.Length > 20)
        {
            return ValidationResult.Invalid("El número de documento debe tener entre 5 y 20 caracteres.");
        }

        return ValidationResult.Valid();
    }
}
