// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class AuthenticationExternalHandler : IAuthenticationExternalHandler
{
    readonly IAuthClient _authClient;
    readonly IIdentityServerInteractionService _interaction;
    readonly IEventService _events;
    readonly IHttpContextAccessor _contextAccessor;
    readonly ISsoEnvironmentProvider _ssoEnvironmentProvider;

    public AuthenticationExternalHandler(IAuthClient authClient, IIdentityServerInteractionService interaction, IEventService events, IHttpContextAccessor contextAccessor, IEnvironmentProvider environmentProvider)
    {
        _authClient = authClient;
        _interaction = interaction;
        _events = events;
        _contextAccessor = contextAccessor;
        _ssoEnvironmentProvider = (ISsoEnvironmentProvider)environmentProvider;
    }

    public async Task<bool> OnHandleAuthenticateAfterAsync(AuthenticateResult result)
    {
        var scheme = result.Properties?.Items?["scheme"] ?? throw new UserFriendlyException("Unknown third party");
        var identity = IdentityProvider.GetIdentity(scheme, result.Principal ?? throw new UserFriendlyException("Authenticate failed"));
        result.Properties.Items.TryGetValue("environment", out var environment);
        environment ??= "development";
        _ssoEnvironmentProvider.SetEnvironment(environment);
        var httpContext = _contextAccessor.HttpContext ?? throw new UserFriendlyException("Internal exception, please contact the administrator");
        var userModel = await _authClient.UserService.GetThirdPartyUserAsync(new GetThirdPartyUserModel
        {
            ThridPartyIdentity = identity.Subject
        });
        if (userModel is not null)
        {
            ProcessLoginCallback(result, out var additionalLocalClaims, out var localSignInProps);
            additionalLocalClaims.Add(new Claim("environment", environment));
            additionalLocalClaims.Add(new Claim("userName", userModel.Account));
            additionalLocalClaims.Add(new Claim("role", JsonSerializer.Serialize(userModel.Roles.Select(role => role.Code))));
            var isuser = new IdentityServerUser(userModel.Id.ToString())
            {
                DisplayName = userModel.DisplayName,
                IdentityProvider = scheme,
                AdditionalClaims = additionalLocalClaims
            };           
            await httpContext.SignInAsync(isuser, localSignInProps);
            await httpContext.SingOutExternalAsync();

            var returnUrl = result.Properties?.Items?["returnUrl"] ?? "~/";
            // check if external login is in the context of an OIDC request
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(userModel.Name, userModel.Id.ToString(), userModel.DisplayName, clientId: context?.Client.ClientId));

            return true;
        }
        else
        {
            httpContext.Response.Redirect($"/account/user/bind");
            return false;
        }
    }

    private void ProcessLoginCallback(AuthenticateResult externalResult, out List<Claim> localClaims, out AuthenticationProperties localSignInProps)
    {
        localClaims = new List<Claim>();
        localSignInProps = new AuthenticationProperties();
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
