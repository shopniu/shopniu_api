
using OpenIddict.Validation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Shopniu_api.Infrastructure.Configuration.Authentication.PermissionsManager;
using Shopniu_api.Aplication.Common.Ports.Identity;
using Shopniu_api.Infrastructure.Services.Users;

namespace Shopniu_api.Infrastructure.Configuration.Authentication;

public static class IdentityServicesExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddAuthorization();

        return services;
    }
}