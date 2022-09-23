// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Middlewares;

public class AuthenticationExternalMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationExternalMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IAuthenticationExternalHandler handler)
    {
        var request = context.Request;
        if (context.Request.Path.Equals(AuthenticationExternalConstants.ChallengeEndpoint, StringComparison.OrdinalIgnoreCase))
        {
            var scheme = request.Query["scheme"];
            var props = new AuthenticationProperties
            {
                RedirectUri = AuthenticationExternalConstants.CallbackEndpoint,
            };
            foreach(var (key,value) in request.Query)
            {
                props.Items.Add(key, value);
            }
            await context.ChallengeAsync(scheme, props);
            return;
        }
        else if (context.Request.Path.Equals(AuthenticationExternalConstants.CallbackEndpoint, StringComparison.OrdinalIgnoreCase))
        {
            var result = await context.AuthenticateAsync(AuthenticationExternalConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }
            var success = await handler.OnHandleAuthenticateAfterAsync(result);
            if(success)
            {
                var returnUrl = result.Properties?.Items?["returnUrl"] ?? "~/";
                context.Response.Redirect(returnUrl);
            }          
            return;
        }

        await _next(context);
    }
}
