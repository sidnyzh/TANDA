using FluentAssertions;
using Tanda.Domain.Entities;
using Tanda.Domain.Services;
using Tanda.Domain.Tests.Fakes;

namespace Tanda.Domain.Tests.Services;

public class CustomerRegistrationServiceTests
{
    [Fact]
    public async Task RegisterAsync_DocumentoValidoYNoExistente_RetornaValidoYAgregaCliente()
    {
        var repository = new FakeCustomerRepository();
        var service = new CustomerRegistrationService(repository);
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FullName = "Juan Pérez",
            DocumentType = "CC",
            DocumentNumber = "1234567",
            Phone = "3001234567"
        };

        var result = await service.RegisterAsync(customer);

        result.IsValid.Should().BeTrue();
        repository.ExistsCallCount.Should().Be(1);
        repository.AddCallCount.Should().Be(1);
    }

    [Fact]
    public async Task RegisterAsync_DocumentoValidoPeroYaRegistrado_RetornaInvalidoYNoAgregaCliente()
    {
        var repository = new FakeCustomerRepository();
        repository.PreloadCustomers([
            new Customer
            {
                Id = Guid.NewGuid(),
                FullName = "Juan Pérez",
                DocumentType = "CC",
                DocumentNumber = "1234567",
                Phone = "3001234567"
            }
        ]);

        var service = new CustomerRegistrationService(repository);
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FullName = "Juan Pérez 2",
            DocumentType = "CC",
            DocumentNumber = "1234567",
            Phone = "3009999999"
        };

        var result = await service.RegisterAsync(customer);

        result.IsValid.Should().BeFalse();
        result.Reason.Should().ContainEquivalentOf("ya existe");
        repository.ExistsCallCount.Should().Be(1);
        repository.AddCallCount.Should().Be(0);
    }

    [Theory]
    [InlineData("XX", "1234567")]
    [InlineData("CC", "12AB5")]
    public async Task RegisterAsync_DocumentoConFormatoInvalido_RetornaInvalidoSinConsultarElRepositorio(string documentType, string documentNumber)
    {
        var repository = new FakeCustomerRepository();
        var service = new CustomerRegistrationService(repository);
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FullName = "Juan Pérez",
            DocumentType = documentType,
            DocumentNumber = documentNumber,
            Phone = "3001234567"
        };

        var result = await service.RegisterAsync(customer);

        result.IsValid.Should().BeFalse();
        repository.ExistsCallCount.Should().Be(0);
        repository.AddCallCount.Should().Be(0);
    }

    [Fact]
    public async Task RegisterAsync_ClienteNulo_LanzaArgumentNullException()
    {
        var repository = new FakeCustomerRepository();
        var service = new CustomerRegistrationService(repository);

        var act = async () => await service.RegisterAsync(null!);

        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
