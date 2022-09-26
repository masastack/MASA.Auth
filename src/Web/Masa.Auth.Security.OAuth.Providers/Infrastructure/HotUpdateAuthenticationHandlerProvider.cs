// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Authentication;

public class HotUpdateAuthenticationHandlerProvider : IAuthenticationHandlerProvider
{
    readonly Dictionary<string, IAuthenticationHandler> _handlerMap = new(StringComparer.Ordinal);
    readonly IAuthenticationSchemeProvider _schemeProvider;
    readonly IRemoteAuthenticationDefaultsProvider _authenticationDefaultsProvider;

    public HotUpdateAuthenticationHandlerProvider(
        IAuthenticationSchemeProvider schemeProvider,
        IRemoteAuthenticationDefaultsProvider authenticationDefaultsProvider)
    {
        _schemeProvider = schemeProvider;
        _authenticationDefaultsProvider = authenticationDefaultsProvider;
    }

    public async Task<IAuthenticationHandler?> GetHandlerAsync(HttpContext context, string authenticationScheme)
    {
        if (_handlerMap.TryGetValue(authenticationScheme, out var value))
            return value;

        var scheme = await _schemeProvider.GetSchemeAsync(authenticationScheme);
        if (scheme is null)
            return null;

        var handler = context.RequestServices.GetService(scheme.HandlerType) as IAuthenticationHandler;
        if (handler is null)
        {
            var authenticationDefaults = await _authenticationDefaultsProvider.GetAsync(scheme.Name);
            if(authenticationDefaults is not null)
            {
                handler = context.RequestServices.GetInstance(authenticationDefaults);
            }          
        }

        if (handler is not null)
        {
            try
            {
                await handler.InitializeAsync(scheme, context);
            }
            catch 
            { 
            }          
            _handlerMap[authenticationScheme] = handler;
        }
        return handler;
    }
}
