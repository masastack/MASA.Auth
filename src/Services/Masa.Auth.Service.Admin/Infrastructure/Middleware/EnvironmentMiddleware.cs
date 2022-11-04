// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class EnvironmentMiddleware : IMiddleware, IScopedDependency
{
    readonly IMultiEnvironmentUserContext _multiEnvironmentUserContext;
    readonly ILogger<EnvironmentMiddleware> _logger;

    public EnvironmentMiddleware(IMultiEnvironmentUserContext multiEnvironmentUserContext, ILogger<EnvironmentMiddleware> logger)
    {
        _multiEnvironmentUserContext = multiEnvironmentUserContext;
        _logger = logger;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation("----- Current Environment Is [{0}]", _multiEnvironmentUserContext.Environment);
        context.Items.Add(IsolationConsts.ENVIRONMENT, _multiEnvironmentUserContext.Environment);
        return next(context);
    }
}
