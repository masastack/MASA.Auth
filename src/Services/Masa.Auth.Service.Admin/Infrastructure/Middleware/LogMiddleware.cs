// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware
{
    public class LogMiddleware<TEvent> : IMiddleware<TEvent>
        where TEvent : notnull, IEvent
    {
        private readonly ILogger<LogMiddleware<TEvent>> _logger;

        public LogMiddleware(ILogger<LogMiddleware<TEvent>> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(TEvent action, EventHandlerDelegate next)
        {
            var typeName = action.GetType().FullName;

            _logger.LogInformation("----- command {CommandType}", typeName);

            await next();
        }
    }
}