using Shopniu_api.Aplication.Common.Ports.Users;
using Shopniu_api.Aplication.Products.Ports;
using Shopniu_api.Infrastructure.ExternalServices.Catalog;
using Shopniu_api.Infrastructure.ExternalServices.Users;

namespace Shopniu_api.Infrastructure.Configuration.Authentication;

public static class ServiceClientsExtensions
{
    public static IServiceCollection AddIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IUserApiClient, UserApiClient>(client =>
        {
            client.BaseAddress = new Uri(configuration["Identity:Issuer"]!);
        });

        services.AddHttpClient<IProductUrlExtractor, JsonLdProductUrlExtractor>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(configuration.GetValue("CatalogExtraction:TimeoutSeconds", 10));
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                configuration.GetValue("CatalogExtraction:UserAgent", "Mozilla/5.0 (compatible; ShopNiu/1.0)"));
        });

        return services;
    }
}