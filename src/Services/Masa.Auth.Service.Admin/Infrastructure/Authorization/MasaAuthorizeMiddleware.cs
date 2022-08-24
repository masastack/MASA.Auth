// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

/// <summary>
/// all route into this,and all RequestDelegateFactory create route handler will Authorization code
/// </summary>
public class MasaAuthorizeMiddleware : IMiddleware
{
    readonly IMasaAuthorizeDataProvider _masaAuthorizeDataProvider;
    readonly EndpointRowDataProvider _endpointRowDataProvider;

    public MasaAuthorizeMiddleware(IMasaAuthorizeDataProvider masaAuthorizeDataProvider,
        EndpointRowDataProvider endpointRowDataProvider)
    {
        _masaAuthorizeDataProvider = masaAuthorizeDataProvider;
        _endpointRowDataProvider = endpointRowDataProvider;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();
        var routeEndpoint = endpoint as RouteEndpoint;
        if (routeEndpoint == null)
        {
            return next(context);
        }
        if (!_endpointRowDataProvider.Endpoints.Contains(routeEndpoint.RoutePattern.RawText))
        {
            return next(context);
        }
        //todo repeat code from CodeAuthorizationMiddlewareResultHandler
        var allowAnonymousAttribute = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
        if (endpoint != null && allowAnonymousAttribute == null)
        {
            var masaAuthorizeAttribute = endpoint?.Metadata.GetMetadata<MasaAuthorizeAttribute>();
            if (masaAuthorizeAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(masaAuthorizeAttribute.Account)
                    && !masaAuthorizeAttribute.Account.Equals(_masaAuthorizeDataProvider.GetAccountAsync().Result))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }
            }
            var code = masaAuthorizeAttribute?.Code;
            if (string.IsNullOrWhiteSpace(code))
            {
                //dafault code rule
                code = Regex.Replace(context.Request.Path, @"\\", ".");
                code = Regex.Replace(code, "/", ".").Trim('.');
                //todo replace MasaStackConsts.AUTH_SYSTEM_SERVICE_APP_ID
                code = $"{MasaStackConsts.AUTH_SYSTEM_SERVICE_APP_ID}.{code}";
            }
            if (!WildCardContainsCode(_masaAuthorizeDataProvider.GetAllowCodeListAsync(MasaStackConsts.AUTH_SYSTEM_SERVICE_APP_ID).Result, code))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        }
        return next(context);

        bool WildCardContainsCode(IEnumerable<string> data, string code)
        {
            return data.Any(item => Regex.IsMatch(code.ToLower(),
                Regex.Escape(item.ToLower()).Replace(@"\*", ".*").Replace(@"\?", ".")));
        }
    }
}
