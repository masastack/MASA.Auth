// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class MasaAuthorizeMiddleware : IMiddleware
{
    readonly IMasaAuthorizeDataProvider _masaAuthorizeDataProvider;
    readonly IEnumerable<EndpointDataSource> _endpointSources;

    public MasaAuthorizeMiddleware(IMasaAuthorizeDataProvider masaAuthorizeDataProvider, IEnumerable<EndpointDataSource> endpointSources)
    {
        _masaAuthorizeDataProvider = masaAuthorizeDataProvider;
        _endpointSources = endpointSources;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var d = _endpointSources.SelectMany(source => source.Endpoints);
        var endpoint = context.GetEndpoint();
        var allowAnonymousAttribute = endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>();
        if (endpoint != null && allowAnonymousAttribute == null)
        {
            var masaAuthorizeAttribute = endpoint?.Metadata.GetMetadata<MasaAuthorizeAttribute>();
            if (masaAuthorizeAttribute != null)
            {
                if (!string.IsNullOrWhiteSpace(masaAuthorizeAttribute.Account)
                    && !masaAuthorizeAttribute.Account.Equals(_masaAuthorizeDataProvider.GetAccount()))
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
            if (!WildCardContainsCode(_masaAuthorizeDataProvider.GetAllowCodeList(), code))
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
