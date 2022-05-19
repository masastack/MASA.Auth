// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages;

public class _HostAuthModel : PageModel
{
    readonly SsoAuthenticationStateCache _ssoAuthenticationStateCache;
    readonly IIdentityServerInteractionService _identityServerInteractionService;
    readonly IDeviceFlowInteractionService _deviceFlowInteractionService;

    public _HostAuthModel(SsoAuthenticationStateCache ssoAuthenticationStateCache,
        IIdentityServerInteractionService identityServerInteractionService,
        IDeviceFlowInteractionService deviceFlowInteractionService)
    {
        _ssoAuthenticationStateCache = ssoAuthenticationStateCache;
        _identityServerInteractionService = identityServerInteractionService;
        _deviceFlowInteractionService = deviceFlowInteractionService;
    }

    public async Task<IActionResult> OnGet()
    {
        var dd = HttpContext.Request;
        switch (HttpContext.Request.Path.Value?.ToLower())
        {
            case "/account/login/index":
            case "/account/login":
                if (HttpContext.Request.Query.ContainsKey("ReturnUrl"))
                {
                    var returnUrl = HttpContext.Request.Query["ReturnUrl"];
                    if (!_ssoAuthenticationStateCache.HasAuthorizationRequest(returnUrl))
                    {
                        _ssoAuthenticationStateCache.AddAuthorizationContext(returnUrl, await _identityServerInteractionService.GetAuthorizationContextAsync(returnUrl));
                    }
                }
                break;
            case "/account/logout/index":
            case "/account/logout":
                if (HttpContext.Request.Query.ContainsKey("LogoutId"))
                {
                    var logoutId = HttpContext.Request.Query["LogoutId"];
                    if (!_ssoAuthenticationStateCache.HasLogoutRequest(logoutId))
                    {
                        _ssoAuthenticationStateCache.AddLogoutRequest(logoutId, await _identityServerInteractionService.GetLogoutContextAsync(logoutId));
                    }
                }
                _ssoAuthenticationStateCache.LogoutId = await _identityServerInteractionService.CreateLogoutContextAsync();
                break;
            case "/grants":
            case "/grants/index":
                _ssoAuthenticationStateCache.Grants = await _identityServerInteractionService.GetAllUserGrantsAsync();
                break;
        }
        Debug.WriteLine($"\n_Host OnGet IsAuth? {User.Identity?.IsAuthenticated}");

        return Page();
    }
}
