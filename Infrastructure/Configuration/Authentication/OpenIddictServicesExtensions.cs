
using OpenIddict.Validation.SystemNetHttp;

public static class OpenIddictServicesExtensions
{
    public static IServiceCollection AddOpenIddictServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenIddict()
            .AddValidation(options =>
            {
                options.SetIssuer(configuration["Identity:Issuer"]!); // URL de shopniu-identity
                options.UseSystemNetHttp();
                options.UseAspNetCore();
            });
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")   // necesita acceso a IHostEnvironment
        {
            services.Configure<OpenIddictValidationSystemNetHttpOptions>(options =>
            {
                options.HttpClientActions.Add(client => { });
                options.HttpClientHandlerActions.Add(handler =>
                {
                    handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                });
            });
        }
        return services;
    }
}