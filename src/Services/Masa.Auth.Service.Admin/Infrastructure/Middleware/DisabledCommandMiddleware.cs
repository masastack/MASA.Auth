// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class DisabledCommandMiddleware<TEvent> : Middleware<TEvent>
    where TEvent : notnull, IEvent
{
    readonly ILogger<DisabledCommandMiddleware<TEvent>> _logger;
    readonly IUserContext _userContext;
    readonly IHostEnvironment _hostEnvironment;

    public DisabledCommandMiddleware(
        ILogger<DisabledCommandMiddleware<TEvent>> logger,
        IUserContext userContext,
        IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _userContext = userContext;
        _hostEnvironment = hostEnvironment;
    }

    public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
    {
        var user = _userContext.GetUser<MasaUser>();
        //todo IsProduction
        if (_hostEnvironment.IsStaging() && user?.Account == "Guest" && @event is ICommand)
        {
            _logger.LogWarning("Guest operation");
            throw new UserFriendlyException("演示账号禁止操作");
        }
        await next();
    }
}