
using Shopniu_api.Aplication.Common.Ports.Users;
using Shopniu_shared.Aplication.Contracts.Identity;

namespace Shopniu_api.Infrastructure.ExternalServices.Users;

public class UserApiClient : IUserApiClient
{
    private readonly HttpClient _httpClient;

    public UserApiClient(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<UserInfoResponse?> GetUserByIdAsync(int userId, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync($"api/v1/users/{userId}", ct);

        if (!response.IsSuccessStatusCode)
            return null;   // se resuelve con null en vez de excepción: el llamador decide el fallback

        return await response.Content.ReadFromJsonAsync<UserInfoResponse>(ct);
    }
}