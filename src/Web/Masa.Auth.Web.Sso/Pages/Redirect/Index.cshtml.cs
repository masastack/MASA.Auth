// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerHost.Pages.Redirect;

[AllowAnonymous]
public class IndexModel : PageModel
{
    public string RedirectUri { get; set; } = string.Empty;

    public IActionResult OnGet(string redirectUri)
    {
        if (!Url.IsLocalUrl(redirectUri))
        {
            return RedirectToPage("/Error/Index");
        }

        RedirectUri = redirectUri;
        return Page();
    }
}