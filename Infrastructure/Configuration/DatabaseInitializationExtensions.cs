using Microsoft.EntityFrameworkCore;
using Shopniu_api.Infrastructure.Persistance;
using Shopniu_api.Infrastructure.Persistance.Seeders;

namespace Shopniu_api.Infrastructure.Configuration;

public static class DatabaseInitializationExtensions
{
    public static async Task<WebApplication> InitializeDatabaseAsync(this WebApplication app)
    {
        var migrateOnStartup = app.Configuration.GetValue<bool>("Database:Migration:RunOnStartup");
        var seedOnStartup = app.Configuration.GetValue<bool>("Database:Seeding:RunOnStartup");

        if (!migrateOnStartup && !seedOnStartup)
        {
            return app;
        }

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (migrateOnStartup)
        {
            await dbContext.Database.MigrateAsync();
        }

        if (seedOnStartup)
        {
            await ProductSeeder.SeedAsync(dbContext, app.Configuration);
        }

        return app;
    }
}
