// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class AuthenticationExternalExtensions
{
    public static async Task ChallengeExternal(this HttpContext context,string scheme,string returnUrl)
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = AuthenticationExternalConstants.CallbackEndpoint,
            Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", scheme },
                }
        };
        await context.ChallengeAsync(scheme, props);
    }

    public static async Task SingOutExternalAsync(this HttpContext context)
    {
        await context.SignOutAsync(AuthenticationExternalConstants.ExternalCookieAuthenticationScheme);
    }

    public static async Task<Identity> GetExternalIdentityAsync(this HttpContext context)
    {
        var result = await context.AuthenticateAsync(AuthenticationExternalConstants.ExternalCookieAuthenticationScheme);
        var scheme = result.Properties?.Items?["scheme"] ?? throw new UserFriendlyException("Unknown third party");
        var identity = IdentityProvider.GetIdentity(scheme, result.Principal ?? throw new UserFriendlyException("Authenticate failed"));
        return identity;
    }
}
