

using Shopniu_shared.Aplication.Contracts.Identity;

namespace Shopniu_api.Aplication.Common.Ports.Users;

public interface IUserApiClient
{
    Task<UserInfoResponse?> GetUserByIdAsync(int userId, CancellationToken ct = default);
}