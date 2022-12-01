// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class DisabledRouteMiddleware : IMiddleware, IScopedDependency
{
    readonly IHostEnvironment _hostEnvironment;

    public DisabledRouteMiddleware(IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //todo IsDemo
        if (_hostEnvironment.IsProduction())
        {
            var endpoint = context.GetEndpoint();
            var disabledRouteAttribute = (endpoint as RouteEndpoint)?.Metadata
                .GetMetadata<DemoDisabledRouteAttribute>();
            if (disabledRouteAttribute != null)
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.DEMO_ENVIRONMENT_FORBIDDEN);
            }
        }
        return next(context);
    }
}
