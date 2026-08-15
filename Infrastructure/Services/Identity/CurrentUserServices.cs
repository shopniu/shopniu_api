// Infrastructure/Services/CurrentUserService.cs
using System.Security.Claims;
using OpenIddict.Abstractions;
using Shopniu_api.Aplication.Common.Ports.Identity;

namespace Shopniu_api.Infrastructure.Services.Users;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User =>
        _httpContextAccessor.HttpContext?.User;

    /// <summary>0 si no hay usuario autenticado (guest checkout).</summary>
    public int UserId
    {
        get
        {
            var subject = User?.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
            return int.TryParse(subject, out var id) ? id : 0;
        }
    }

    public string? Email => User?.FindFirst(OpenIddictConstants.Claims.Email)?.Value;

    public string? Name => User?.FindFirst(OpenIddictConstants.Claims.Name)?.Value;
}
