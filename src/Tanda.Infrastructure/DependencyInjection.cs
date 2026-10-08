using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tanda.Domain.Abstractions;
using Tanda.Infrastructure.Configuration;
using Tanda.Infrastructure.Persistence;
using Tanda.Infrastructure.Repositories;

namespace Tanda.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<TandaDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddOptions<BusinessOptions>()
            .Bind(configuration.GetSection(BusinessOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IBusinessParameters, BusinessParametersProvider>();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISizeRepository, SizeRepository>();
        services.AddScoped<IAdditionRepository, AdditionRepository>();
        services.AddScoped<IDailyCapacityRepository, DailyCapacityRepository>();

        return services;
    }
}