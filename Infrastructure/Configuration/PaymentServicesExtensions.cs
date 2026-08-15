using Microsoft.Extensions.Options;
using Shopniu_api.Aplication.Payments.Ports;
using Shopniu_api.Aplication.Transactions.Ports;
using Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi;
using Shopniu_api.Infrastructure.ExternalServices.Payment.Wompi.Services;

namespace Shopniu_api.Infrastructure.Configuration;

public static class PaymentServicesExtensions
{
    public static IServiceCollection AddPaymentServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<WompiSettings>()
            .Bind(configuration.GetSection(WompiSettings.SectionName))
            .Validate(settings =>
                !string.IsNullOrWhiteSpace(settings.IntegrityKey) &&
                !string.IsNullOrWhiteSpace(settings.PublicKey) &&
                !string.IsNullOrWhiteSpace(settings.PrivateKey) &&
                !string.IsNullOrWhiteSpace(settings.EventsKey) &&
                Uri.TryCreate(settings.ApiUrl, UriKind.Absolute, out _),
                $"'{WompiSettings.SectionName}' configuration is invalid. Review appsettings or environment variables.")
            .ValidateOnStart();

        services.AddScoped<WompiSignatureGenerator>();
        services.AddScoped<IPaymentWebhookTranslator, WompiWebhookTranslator>();
        services.AddScoped<WompiWebhookSignatureValidation>();

        services.AddHttpClient<IPaymentGateway, WompiPaymentGateway>((serviceProvider, client) =>
        {
            var wompiSettings = serviceProvider.GetRequiredService<IOptions<WompiSettings>>().Value;
            client.BaseAddress = new Uri(wompiSettings.ApiUrl);
            client.Timeout = TimeSpan.FromSeconds(15);
        })
        // No reutilizar conexiones: reduce el reintento por "conexión stale" en
        // la pasarela de pagos (reintentar un POST duplicaría la referencia).
        .ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.Zero
        });

        return services;
    }
}
