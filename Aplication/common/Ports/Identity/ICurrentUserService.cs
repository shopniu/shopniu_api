// Aplication/Common/Ports/ICurrentUserService.cs
namespace Shopniu_api.Aplication.Common.Ports.Identity;

public interface ICurrentUserService
{
    /// <summary>ID del usuario autenticado; 0 si la petición es anónima.</summary>
    int UserId { get; }
    string? Email { get; }
    string? Name { get; }
}
