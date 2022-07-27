// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages;

public class _HostAuthModel : PageModel
{
    readonly SsoAuthenticationStateCache _ssoAuthenticationStateCache;
    readonly IIdentityServerInteractionService _identityServerInteractionService;

    public _HostAuthModel(SsoAuthenticationStateCache ssoAuthenticationStateCache,
        IIdentityServerInteractionService identityServerInteractionService)
    {
        _ssoAuthenticationStateCache = ssoAuthenticationStateCache;
        _identityServerInteractionService = identityServerInteractionService;
    }

    public async Task<IActionResult> OnGet()
    {
        switch (HttpContext.Request.Path.Value?.ToLower())
        {
            case "/account/login/index":
            case "/account/login":
            case "/consent":
            case "/consent/index":
                if (HttpContext.Request.Query.ContainsKey("ReturnUrl"))
                {
                    var returnUrl = HttpContext.Request.Query["ReturnUrl"];
                    var d = await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl);
                    if (!_ssoAuthenticationStateCache.HasAuthorizationRequest(returnUrl))
                    {
                        _ssoAuthenticationStateCache.AddAuthorizationContext(returnUrl, await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl));
                    }
                }
                break;
            case "/grants":
            case "/grants/index":
                _ssoAuthenticationStateCache.Grants = await _identityServerInteractionService.GetAllUserGrantsAsync();
                break;
            default:
                break;
        }
        Debug.WriteLine($"\n_Host OnGet IsAuth? {User.Identity?.IsAuthenticated}");

        return Page();
    }
}
