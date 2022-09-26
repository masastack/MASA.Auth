// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.JSInterop;

namespace Masa.Auth.Security.OAuth.Providers.Infrastructure;

public class CookieAuthenticationHandler : Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationHandler
{
    public CookieAuthenticationHandler(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task InitializeHandlerAsync()
    {
        var jsRuntime = Context.RequestServices.GetService<IJSRuntime>();
        if (jsRuntime is null)
        {
            Context.Response.OnStarting(FinishResponseAsync);
        }
        return Task.CompletedTask;
    }
}
