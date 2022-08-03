// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Service.Admin;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Masa.Auth.Web.Admin.Server.Pages;

public class _HostAuthModel : PageModel
{
    readonly BlazorServerTokenCache _blazorServerTokenCache;

    public _HostAuthModel(BlazorServerTokenCache blazorServerTokenCache)
    {
        _blazorServerTokenCache = blazorServerTokenCache;
    }

    public async Task<IActionResult> OnGet()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var sid = User.Claims
                    .Where(c => c.Type.Equals("sub"))
                    .Select(c => c.Value)
                    .FirstOrDefault();
            if (!string.IsNullOrEmpty(sid))
            {
                var tokenData = new BlazorServerTokenData
                {
                    AccessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken),
                    RefreshToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken),
                    IdToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken)
                };
                _blazorServerTokenCache.Add(sid, tokenData);
            }
        }
        return Page();
    }
}
