// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class EnvironmentMiddleware : IMiddleware, IScopedDependency
{
    readonly IEnvironmentContext _environmentContext;
    readonly ILogger<EnvironmentMiddleware> _logger;

    public EnvironmentMiddleware(IEnvironmentContext environmentContext,
        ILogger<EnvironmentMiddleware> logger)
    {
        _environmentContext = environmentContext;
        _logger = logger;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation("----- Current Environment Is [{0}]", _environmentContext.CurrentEnvironment);
        context.Items.Add(IsolationConsts.ENVIRONMENT, _environmentContext.CurrentEnvironment);
        return next(context);
    }
}
