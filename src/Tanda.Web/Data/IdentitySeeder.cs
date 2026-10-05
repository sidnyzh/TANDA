using Microsoft.AspNetCore.Identity;

namespace Tanda.Web.Data;

public static class IdentitySeeder
{
    public static readonly string[] Roles = ["Administrator", "Salesperson", "Baker"];

    public static async Task SeedRolesAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(IdentitySeeder));

        foreach (var role in Roles)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(role));

            if (result.Succeeded)
            {
                logger.LogInformation("Role '{Role}' created.", role);
            }
            else
            {
                logger.LogError(
                    "Could not create role '{Role}': {Errors}",
                    role,
                    string.Join("; ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}