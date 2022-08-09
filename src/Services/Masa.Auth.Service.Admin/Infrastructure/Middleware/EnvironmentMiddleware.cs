// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class EnvironmentMiddleware : IMiddleware
{
    readonly IMultiEnvironmentUserContext _multiEnvironmentUserContext;

    public EnvironmentMiddleware(IMultiEnvironmentUserContext multiEnvironmentUserContext)
    {
        _multiEnvironmentUserContext = multiEnvironmentUserContext;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Items.Add(IsolationConsts.ENVIRONMENT, _multiEnvironmentUserContext.Environment);
        return next(context);
    }
}
