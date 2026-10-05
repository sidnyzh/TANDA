using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tanda.Domain.Abstractions;
using Tanda.Infrastructure.Configuration;

namespace Tanda.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<BusinessOptions>()
            .Bind(configuration.GetSection(BusinessOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IBusinessParameters, BusinessParametersProvider>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}