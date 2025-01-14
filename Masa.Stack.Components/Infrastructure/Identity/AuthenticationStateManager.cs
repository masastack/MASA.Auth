using Microsoft.AspNetCore.Components.Authorization;

namespace Masa.Stack.Components.Infrastructure.Identity;

public class AuthenticationStateManager : IScopedDependency
{
    private readonly CustomAuthenticationStateProvider _authenticationStateProvider;

    public AuthenticationStateManager(CustomAuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
    }

    public async Task UpsertClaimAsync(string key, string value)
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var identity = user.Identity as ClaimsIdentity;
        if (identity == null)
            return;

        // Check for existing claim and remove it
        var existingClaim = identity.FindFirst(key);
        if (existingClaim != null)
            identity.RemoveClaim(existingClaim);

        // Add new claim
        identity.AddClaim(new Claim(key, value));

        // Notify the authentication state provider of the updated user
        _authenticationStateProvider.NotifyUserAuthentication(user);
    }
}

// Custom AuthenticationStateProvider
public class CustomAuthenticationStateProvider : AuthenticationStateProvider, IScopedDependency
{
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public void NotifyUserAuthentication(ClaimsPrincipal user)
    {
        _currentUser = user;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }
}