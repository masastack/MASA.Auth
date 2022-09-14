// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class CreateAuthenticationHandlerInstanceUtilities
{
    public static (IOptionsMonitor<Options> options, ILoggerFactory loggerFactory, UrlEncoder urlEncoder, ISystemClock systemClock) BuilderParamter<Options>(IServiceProvider provider) where Options : AuthenticationSchemeOptions
    {
        var options = provider.GetRequiredService<IOptionsMonitor<Options>>();
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var urlEncoder = provider.GetRequiredService<UrlEncoder>();
        var systemClock = provider.GetRequiredService<ISystemClock>();
        return (options, loggerFactory, urlEncoder, systemClock);
    }
}
