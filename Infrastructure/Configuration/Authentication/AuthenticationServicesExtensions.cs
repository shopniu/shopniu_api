
namespace Shopniu_api.Infrastructure.Configuration.Authentication;

public static class AuthenticationServicesExtensions
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration
        )
    {
        services
            .AddIdentityServices()
            .AddOpenIddictServices(configuration)
            .AddAuthorization();

        return services;
    }
}