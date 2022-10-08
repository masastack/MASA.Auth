// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public class CookieAuthenticationHandler : Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationHandler
{
    public CookieAuthenticationHandler(IOptionsMonitor<CookieAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override Task InitializeHandlerAsync()
    {
        if (Context.Response.HasStarted is false)
        {
            Context.Response.OnStarting(FinishResponseAsync);
        }
        return Task.CompletedTask;
    }
}
