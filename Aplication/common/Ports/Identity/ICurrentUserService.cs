// Aplication/Common/Ports/ICurrentUserService.cs
namespace Shopniu_api.Aplication.Common.Ports.Identity;

public interface ICurrentUserService
{
    int UserId { get; }
    string Email { get; }
    string Name { get; }
}