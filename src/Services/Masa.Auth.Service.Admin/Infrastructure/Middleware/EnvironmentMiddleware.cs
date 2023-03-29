// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class EnvironmentMiddleware : IMiddleware, IScopedDependency
{
    readonly IMultiEnvironmentContext _multiEnvironmentContext;
    readonly ILogger<EnvironmentMiddleware> _logger;

    public EnvironmentMiddleware(IMultiEnvironmentContext multiEnvironmentContext,
        ILogger<EnvironmentMiddleware> logger)
    {
        _multiEnvironmentContext = multiEnvironmentContext;
        _logger = logger;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation("============Headers:{0}=============", context.Request.Headers);
        _logger.LogInformation("----- Current Environment Is [{0}] -----", _multiEnvironmentContext.CurrentEnvironment);
        context.Items.Add(IsolationConsts.ENVIRONMENT, _multiEnvironmentContext.CurrentEnvironment);
        return next(context);
    }
}
