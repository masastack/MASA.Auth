// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Service.Admin.Infrastructure.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class CodeAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        //Enhance the default challenge or forbid responses.
        //context.Response.StatusCode = StatusCodes.Status403Forbidden;
        var allowAnonymousAttribute = next.GetMethodInfo().GetCustomAttribute<AllowAnonymousAttribute>();
        if (allowAnonymousAttribute == null)
        {
            var masaAuthorizeAttribute = next.GetMethodInfo().GetCustomAttribute<MasaAuthorizeAttribute>();
        }
        await defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}
