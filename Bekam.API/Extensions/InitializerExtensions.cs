using Bekam.Application.Abstraction.Contracts.Persistence.DbInitializers;

namespace Bekam.API.Extensions;

public static class InitializerExtensions
{
    public static async Task<WebApplication> InitializeDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;

        var storeContextInitializer = services.GetRequiredService<IAppDbInitializer>();
        var identityContextInitializer = services.GetRequiredService<IIdentityDbInitializer>();

        var loggerFactory = services.GetRequiredService<ILoggerFactory>();

        try
        {
            await storeContextInitializer.InitializeAsync();
            await storeContextInitializer.SeedAsync();

            await identityContextInitializer.InitializeAsync();
            await identityContextInitializer.SeedAsync();

        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<Program>();
            logger.LogError(ex, "an error has been occurred during applying the migrations or the data seeding.");
        }

        return app;
    }
}