

using Microsoft.AspNetCore.Authorization;

namespace Shopniu_api.Infrastructure.Configuration.Authentication.PermissionsManager;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}