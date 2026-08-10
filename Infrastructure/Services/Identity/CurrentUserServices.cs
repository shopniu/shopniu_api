// Infrastructure/Services/CurrentUserService.cs
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

    private System.Security.Claims.ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("No hay usuario autenticado en el contexto actual.");

    public int UserId =>
        int.Parse(User.FindFirst(OpenIddictConstants.Claims.Subject)!.Value);

    public string Email =>
        User.FindFirst(OpenIddictConstants.Claims.Email)!.Value;

    public string Name =>
        User.FindFirst(OpenIddictConstants.Claims.Name)!.Value;
}