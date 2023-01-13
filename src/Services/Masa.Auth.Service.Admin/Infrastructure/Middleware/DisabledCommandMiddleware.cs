// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class DisabledCommandMiddleware<TEvent> : Middleware<TEvent>
    where TEvent : notnull, IEvent
{
    readonly ILogger<DisabledCommandMiddleware<TEvent>> _logger;
    readonly IUserContext _userContext;
    readonly IMasaStackConfig _masaStackConfig;

    public DisabledCommandMiddleware(
        ILogger<DisabledCommandMiddleware<TEvent>> logger,
        IUserContext userContext,
        IMasaStackConfig masaStackConfig)
    {
        _logger = logger;
        _userContext = userContext;
        _masaStackConfig = masaStackConfig;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var user = _userContext.GetUser<MasaUser>();
        if (_masaStackConfig.IsDemo && user?.Account == "Guest" && @event is ICommand)
        {
            _logger.LogWarning("Guest operation");
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.GUEST_ACCOUNT_OPERATE);
        }
        await next();
    }
}