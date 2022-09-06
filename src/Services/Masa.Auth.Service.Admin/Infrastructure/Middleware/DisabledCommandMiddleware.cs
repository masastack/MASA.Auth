// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware
{
    public class DisabledCommandMiddleware<TEvent> : Middleware<TEvent>
        where TEvent : notnull, IEvent
    {
        readonly IUserContext _userContext;

        public DisabledCommandMiddleware(IUserContext userContext)
        {
            _userContext = userContext;
        }

        public override async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
        {
            if (_userContext.UserName == "Guest" && @event is ICommand)
            {
                throw new UserFriendlyException("演示账号禁止操作");
            }
            await next();
        }
    }
}