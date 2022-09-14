// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.AspNetCore.Authentication;

public class HotUpdateAuthenticationHandlerProvider : IAuthenticationHandlerProvider
{
    private readonly Dictionary<string, IAuthenticationHandler> _handlerMap = new(StringComparer.Ordinal);

    public IAuthenticationSchemeProvider Schemes { get; }

    public HotUpdateAuthenticationHandlerProvider(IAuthenticationSchemeProvider schemes)
    {
        Schemes = schemes;
    }

    public async Task<IAuthenticationHandler?> GetHandlerAsync(HttpContext context, string authenticationScheme)
    {
        if (_handlerMap.TryGetValue(authenticationScheme, out var value))
        {
            return value;
        }
        var scheme = await Schemes.GetSchemeAsync(authenticationScheme);
        if (scheme is null)
        {
            return null;
        }
        var handler = (context.RequestServices.GetService(scheme.HandlerType) ??
            ActivatorUtilities.CreateInstance(context.RequestServices, scheme.HandlerType))
            as IAuthenticationHandler;
        // todo support custom oauth handler createInstance
        if (handler != null)
        {
            await handler.InitializeAsync(scheme, context);
            _handlerMap[authenticationScheme] = handler;
        }
        return handler;
    }
}
