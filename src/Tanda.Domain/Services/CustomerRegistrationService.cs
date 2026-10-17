using Tanda.Domain.Abstractions;
using Tanda.Domain.Common;
using Tanda.Domain.Entities;
using Tanda.Domain.Rules;

namespace Tanda.Domain.Services;

public sealed class CustomerRegistrationService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerRegistrationService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<ValidationResult> RegisterAsync(
        Customer customer,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);

        var documentValidation = CustomerDocumentValidator.Validate(customer.DocumentType, customer.DocumentNumber);
        if (!documentValidation.IsValid)
        {
            return documentValidation;
        }

        var exists = await _customerRepository.ExistsByDocumentAsync(
            customer.DocumentType,
            customer.DocumentNumber,
            cancellationToken);

        if (exists)
        {
            return ValidationResult.Invalid("Ya existe un cliente con ese documento.");
        }

        await _customerRepository.AddAsync(customer, cancellationToken);
        return ValidationResult.Valid();
    }
}
