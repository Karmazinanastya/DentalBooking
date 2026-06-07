using System.Net.Http.Headers;
using System.Net.Http.Json;
using AdminWebApp.Models;

namespace AdminWebApp.Services;

public sealed class IdentityApiClient(IHttpClientFactory httpClientFactory, AuthState authState)
{
    private HttpClient CreateClient()
    {
        var client = httpClientFactory.CreateClient("identity");
        if (!string.IsNullOrEmpty(authState.AccessToken))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authState.AccessToken);
        return client;
    }

    public async Task<(bool Success, LoginResponse? Data, string? Error)> LoginAsync(LoginRequest request)
    {
        try
        {
            var client = httpClientFactory.CreateClient("identity");
            var response = await client.PostAsJsonAsync("auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return (true, data, null);
            }
            var err = await response.Content.ReadAsStringAsync();
            return (false, null, err);
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public async Task<List<UserDto>?> GetUsersAsync()
    {
        var client = CreateClient();
        return await client.GetFromJsonAsync<List<UserDto>>("auth/users");
    }

    public async Task<(bool Success, string? Error)> RegisterUserAsync(RegisterUserRequest request)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("auth/register", request);
        if (response.IsSuccessStatusCode) return (true, null);
        var err = await response.Content.ReadAsStringAsync();
        return (false, err);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var client = CreateClient();
        var response = await client.DeleteAsync($"auth/users/{id}");
        return response.IsSuccessStatusCode;
    }
}
