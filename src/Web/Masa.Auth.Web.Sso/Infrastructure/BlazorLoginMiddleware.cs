// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class BlazorLoginMiddleware
{
    private readonly RequestDelegate _next;

    public BlazorLoginMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, TestUserStore Users)
    {
        if (context.Request.Path == "/login"
            && context.Request.Query.ContainsKey("user_name")
            && context.Request.Query.ContainsKey("password"))
        {
            var userName = context.Request.Query["user_name"];
            var pwd = context.Request.Query["password"];

            if (Users.ValidateCredentials(userName, pwd))
            {
                var user = Users.FindByUsername(context.Request.Query["user_name"]);
                bool.TryParse(context.Request.Query["remember_login"], out bool rememberLogin);
                // only set explicit expiration here if user chooses "remember me". 
                // otherwise we rely upon expiration configured in cookie middleware.
                AuthenticationProperties? props = null;
                if (LoginOptions.AllowRememberLogin && rememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(LoginOptions.RememberMeLoginDuration)
                    };
                };
                var isuser = new IdentityServerUser(user.SubjectId)
                {
                    DisplayName = user.Username
                };
                //us duende sign in
                await context.SignInAsync(isuser, props);
                await context.Response.WriteAsJsonAsync(user);
            }
            else
            {
                throw new UserFriendlyException("username and password validate error");
            }
            return;
        }
        else if (context.Request.Path.StartsWithSegments("/logout"))
        {
            if (context.Request.Query.ContainsKey("redirect_uri") && context.Request.Query.ContainsKey("idp"))
            {
                var redirect_uri = context.Request.Query["redirect_uri"];
                var idp = context.Request.Query["idp"];
                await context.SignOutAsync(idp, new AuthenticationProperties { RedirectUri = redirect_uri });
            }
            else
            {
                await context.SignOutAsync();
            }
            return;
        }
        //Continue http middleware chain:
        await _next.Invoke(context);
    }
}
