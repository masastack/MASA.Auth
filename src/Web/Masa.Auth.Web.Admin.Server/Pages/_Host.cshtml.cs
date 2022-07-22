// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Masa.Auth.Web.Admin.Server.Pages;

public class _HostModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        var path = HttpContext.Request.Path.Value?.ToLower();
        var cookies = HttpContext.Request.Cookies;
        return Page();
    }
}
