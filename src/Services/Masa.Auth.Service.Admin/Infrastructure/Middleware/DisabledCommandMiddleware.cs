// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware
{
    public class DisabledCommandMiddleware<TEvent> : IMiddleware<TEvent>
        where TEvent : notnull, IEvent
    {
        public async Task HandleAsync(TEvent @event, EventHandlerDelegate next)
        {
            if (@event is ICommand)
            {
                throw new UserFriendlyException("禁止操作");
            }

            await next();
        }
    }
}