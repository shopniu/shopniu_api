using Shopniu_api.Aplication.Common.Ports.Storage;
using Shopniu_api.Infrastructure.Middlewares;

namespace Shopniu_api.Infrastructure.Configuration.Pipeline;

public static class ApiPipelineExtensions
{
    public static async Task<WebApplication> UseApiPipeline(this WebApplication app)
    {
        // Initialize the database if required (seeders and migrations)   
        await app.InitializeDatabaseAsync();

        // En desarrollo el emulador (Azurite) parte vacío: crear el contenedor
        // público de media si no existe. En producción se provisiona en el portal.
        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var storage = scope.ServiceProvider.GetRequiredService<IBlobStorageService>();
            await storage.EnsureContainerExistsAsync();
        }

        app.UseGlobalExceptionHandler();
        app.UseForwardedHeaders();
        app.UseResponseCompression();
        app.UseRateLimiter();


        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();


        // Authentication and Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Map controllers and health checks
        app.MapControllers();
        app.MapHealthChecks("/health");

        return app;
    }
}
