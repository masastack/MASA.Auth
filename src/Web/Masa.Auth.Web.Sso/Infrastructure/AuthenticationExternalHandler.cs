// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class AuthenticationExternalHandler : IAuthenticationExternalHandler
{
    readonly IAuthClient _authClient;
    readonly IIdentityServerInteractionService _interaction;
    readonly IEventService _events;
    readonly IHttpContextAccessor _contextAccessor;

    public AuthenticationExternalHandler(IAuthClient authClient, IIdentityServerInteractionService interaction, IEventService events, IHttpContextAccessor contextAccessor)
    {
        _authClient = authClient;
        _interaction = interaction;
        _events = events;
        _contextAccessor = contextAccessor;
    }

    public async Task<bool> OnHandleAuthenticateAfterAsync(AuthenticateResult result)
    {
        var (provider, providerUserId, claims) = FindUserFromExternalProvider(result);
        // todo add third party user
        var userModel = new UserModel()
        {
            Id = Guid.Parse("9C9A17D6-0B0E-4697-6BD6-08DA706507F0"),
            DisplayName = providerUserId,
        };

        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        ProcessLoginCallback(result, additionalLocalClaims, localSignInProps);

        // issue authentication cookie for user
        var isuser = new IdentityServerUser(userModel.Id.ToString())
        {
            DisplayName = userModel.DisplayName,
            IdentityProvider = provider,
            AdditionalClaims = additionalLocalClaims
        };
        //result.Properties?.Items.TryGetValue("environment", out var environment);
        var environment = "development";
        isuser.AdditionalClaims.Add(new Claim("userName", userModel.Account));
        isuser.AdditionalClaims.Add(new Claim("environment", environment));
        isuser.AdditionalClaims.Add(new Claim("role", JsonSerializer.Serialize(userModel.RoleIds)));

        var httpContext = _contextAccessor.HttpContext ?? throw new UserFriendlyException("Internal exception, please contact the administrator");
        await httpContext.SignInAsync(isuser, localSignInProps);
        await httpContext.SingOutExternalAsync();

        var returnUrl = result.Properties?.Items?["returnUrl"] ?? "~/";
        // check if external login is in the context of an OIDC request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        await _events.RaiseAsync(new UserLoginSuccessEvent(userModel.Name, userModel.Id.ToString(), userModel.DisplayName, clientId: context?.Client.ClientId));
        httpContext.Response.Redirect(returnUrl);

        var ssoEnvironmentProvider = httpContext.RequestServices.GetService<IEnvironmentProvider>() as ISsoEnvironmentProvider;
        if (ssoEnvironmentProvider != null)
        {
            ssoEnvironmentProvider.SetEnvironment(environment);
        }

        return true;
    }

    private (string provider, string providerUserId, IEnumerable<Claim> claims) FindUserFromExternalProvider(AuthenticateResult result)
    {
        var externalUser = result.Principal ?? throw new UserFriendlyException("Get user data failed");

        var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          throw new UserFriendlyException("Unknown user");

        // remove the user id claim so we don't include it as an extra claim if/when we provision the user
        var claims = externalUser.Claims.ToList();
        claims.Remove(userIdClaim);

        var provider = result.Properties?.Items?["scheme"] ?? throw new UserFriendlyException("unknown third party");
        var providerUserId = userIdClaim.Value;

        return (provider, providerUserId, claims);
    }

    private void ProcessLoginCallback(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        var sid = externalResult.Principal?.Claims?.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        var idToken = externalResult.Properties?.GetTokenValue("id_token");
        if (idToken != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
        }
    }
}
