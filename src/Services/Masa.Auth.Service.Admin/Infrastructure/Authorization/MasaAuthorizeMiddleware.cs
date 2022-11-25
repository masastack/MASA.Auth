// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

/// <summary>
/// all route into this,and all RequestDelegateFactory create route handler will Authorization code
/// </summary>
public class MasaAuthorizeMiddleware : IMiddleware, IScopedDependency
{
    readonly IMasaAuthorizeDataProvider _masaAuthorizeDataProvider;
    readonly EndpointRowDataProvider _endpointRowDataProvider;
    readonly ILogger<MasaAuthorizeMiddleware> _logger;

    public MasaAuthorizeMiddleware(IMasaAuthorizeDataProvider masaAuthorizeDataProvider,
        EndpointRowDataProvider endpointRowDataProvider,
        ILogger<MasaAuthorizeMiddleware> logger)
    {
        _masaAuthorizeDataProvider = masaAuthorizeDataProvider;
        _endpointRowDataProvider = endpointRowDataProvider;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var routeEndpoint = endpoint as RouteEndpoint;
        if (routeEndpoint == null)
        {
            await next(context);
            return;
        }
        //exclude unprogrammed route such as dapr 
        if (!_endpointRowDataProvider.Endpoints.Contains(routeEndpoint.RoutePattern.RawText))
        {
            await next(context);
            return;
        }
        var allowAnonymousAttribute = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
        if (endpoint != null && allowAnonymousAttribute == null)
        {
            var masaAuthorizeAttribute = endpoint?.Metadata.GetMetadata<MasaAuthorizeAttribute>();
            if (masaAuthorizeAttribute != null)
            {
                if (masaAuthorizeAttribute.Roles?.Split(',').ToList()
                .Intersect(await _masaAuthorizeDataProvider.GetRolesAsync()).Any() == true)
                {
                    _logger.LogInformation("----- authentication role passed");
                    await next(context);
                    return;
                }
            }
            var code = masaAuthorizeAttribute?.Code;
            if (string.IsNullOrWhiteSpace(code))
            {
                //dafault code rule
                code = Regex.Replace(context.Request.Path, @"\\", ".");
                code = Regex.Replace(code, "/", ".").Trim('.');
                code = $"{MasaStackConsts.AUTH_SYSTEM_SERVICE_APP_ID}.{code}";
            }

            if (!(await _masaAuthorizeDataProvider.GetAllowCodesAsync(MasaStackConsts.AUTH_SYSTEM_SERVICE_APP_ID)).WildCardContains(code))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }
        }
        await next(context);
    }
}

