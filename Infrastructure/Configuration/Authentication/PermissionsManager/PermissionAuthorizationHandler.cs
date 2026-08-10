
using Microsoft.AspNetCore.Authorization;


namespace Shopniu_api.Infrastructure.Configuration.Authentication.PermissionsManager;

public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var hasPermission = context.User.FindAll("permission").Any(p => p.Value == requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}