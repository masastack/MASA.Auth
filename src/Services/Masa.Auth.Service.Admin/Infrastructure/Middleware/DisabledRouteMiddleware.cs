// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class DisabledRouteMiddleware : IMiddleware, IScopedDependency
{
    List<string?> _disabledRoute = new List<string?>()
    {
        "/api/message/sms",
        "/api/message/email"
    };

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var account = context.User.Claims.FirstOrDefault(c => c.Type == "account");
        //todo add demo environment judge
        if (_disabledRoute.Contains(context.Request.Path.Value))
        {
            throw new UserFriendlyException("演示环境禁止操作");
        }
        return next(context);
    }
}
