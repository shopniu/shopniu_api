using Shopniu_api.Aplication.Common.Ports.Storage;
using Shopniu_api.Infrastructure.ExternalServices.Storage;

namespace Shopniu_api.Infrastructure.Configuration;

public static class StorageServicesExtensions
{
    public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<StorageSettings>()
            .Bind(configuration.GetSection(StorageSettings.SectionName))
            .Validate(settings =>
                    !string.IsNullOrWhiteSpace(settings.AccountName) &&
                    !string.IsNullOrWhiteSpace(settings.ContainerName) &&
                    !string.IsNullOrWhiteSpace(settings.PublicBaseUrl),
                $"'{StorageSettings.SectionName}' configuration is invalid. Review appsettings or environment variables.")
            .ValidateOnStart();

        services.AddSingleton<IBlobStorageService, AzureBlobStorageService>();

        return services;
    }
}
