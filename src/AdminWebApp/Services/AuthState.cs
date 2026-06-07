using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AdminWebApp.Models;

namespace AdminWebApp.Services;

public sealed class AuthState
{
    public string? AccessToken { get; private set; }
    public LoginResponse? CurrentUser { get; private set; }
    public bool IsAuthenticated => CurrentUser is not null && !string.IsNullOrEmpty(AccessToken);
    public bool IsAdmin => CurrentUser?.Role == "Admin";
    public bool IsDoctor => CurrentUser?.Role == "Doctor";

    public event Action? OnChange;

    public void SignIn(LoginResponse response)
    {
        AccessToken = response.AccessToken;
        CurrentUser = response;
        NotifyStateChanged();
    }

    public void SignOut()
    {
        AccessToken = null;
        CurrentUser = null;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
