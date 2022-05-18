// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Masa.Auth.Web.Admin.Server.Pages;

public class LoginModel : PageModel
{
    public async Task<IActionResult> OnGetAsync(string redirectUri)
    {
        // just to remove compiler warning
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(redirectUri))
        {
            redirectUri = Url.Content("~/");
        }

        // If user is already logged in, we can redirect directly...
        if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
        {
            Response.Redirect(redirectUri);
        }

        return Challenge(
            new AuthenticationProperties
            {
                RedirectUri = redirectUri
            },
            OpenIdConnectDefaults.AuthenticationScheme);
    }
}
