using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Tanda.Infrastructure.Persistence;

public sealed class TandaDbContextFactory : IDesignTimeDbContextFactory<TandaDbContext>
{
    public TandaDbContext CreateDbContext(string[] args)
    {
        var webProjectPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "src",
            "Tanda.Web");

        if (!Directory.Exists(webProjectPath))
        {
            webProjectPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "Tanda.Web");
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(webProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile(
                "appsettings.Development.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<TandaDbContext>();

        optionsBuilder.UseSqlServer(
            connectionString,
            sqlServer => sqlServer.MigrationsAssembly(
                typeof(TandaDbContext).Assembly.FullName));

        return new TandaDbContext(optionsBuilder.Options);
    }
}