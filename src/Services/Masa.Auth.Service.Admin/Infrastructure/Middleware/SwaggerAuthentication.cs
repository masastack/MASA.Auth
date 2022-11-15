// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class SwaggerAuthentication
{
    private readonly RequestDelegate _next;

    public SwaggerAuthentication(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if(context.Request.Cookies.TryGetValue(BusinessConsts.SWAGGER_TOKEN, out var token))
        {
            context.Request.Headers.Authorization = $"Bearer {token}";
        }

        await _next(context);
    }
}
