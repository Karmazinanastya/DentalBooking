using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace AdminWebApp.Services;

public sealed class CustomAuthStateProvider(AuthState authState) : AuthenticationStateProvider
{
    private static readonly AuthenticationState AnonymousState =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (!authState.IsAuthenticated || authState.CurrentUser is null)
            return Task.FromResult(AnonymousState);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, authState.CurrentUser.UserId.ToString()),
            new(ClaimTypes.Email, authState.CurrentUser.Email),
            new(ClaimTypes.Name, authState.CurrentUser.FullName),
            new(ClaimTypes.Role, authState.CurrentUser.Role),
        };

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        return Task.FromResult(new AuthenticationState(user));
    }

    public void NotifyAuthStateChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
