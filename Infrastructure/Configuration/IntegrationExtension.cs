using Shopniu_api.Aplication.Common.Ports.Users;
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

        return services;
    }
}