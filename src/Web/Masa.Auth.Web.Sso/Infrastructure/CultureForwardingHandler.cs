// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.WebUtilities;

namespace Masa.Auth.Web.Sso.Infrastructure;

/// <summary>
/// 将入站请求中显式携带的语言信息透传给下游服务（当前为 Auth 服务），
/// 使下游返回的 UserFriendlyException 报错文案按请求语言本地化。
/// 仅支持客户端通过 HTTP 显式指定的语言：URL ?culture= 与 Accept-Language 请求头；
/// 请求未携带语言信息时不做处理，由下游服务使用自身默认语言。
/// </summary>
public class CultureForwardingHandler : DelegatingHandler
{
    private const string CultureQueryKey = "culture";
    private const string AcceptLanguageHeaderName = "Accept-Language";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public CultureForwardingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is not null)
        {
            ForwardCulture(httpContext, request);
        }
        return await base.SendAsync(request, cancellationToken);
    }

    private void ForwardCulture(HttpContext httpContext, HttpRequestMessage request)
    {
        if (request.RequestUri is null)
        {
            return;
        }

        // URL ?culture=
        if (httpContext.Request.Query.TryGetValue(CultureQueryKey, out var cultureQuery) && !string.IsNullOrEmpty(cultureQuery))
        {
            request.RequestUri = AppendCultureQuery(request.RequestUri, cultureQuery.ToString());
        }

        // Accept-Language 请求头
        if (request.Headers.AcceptLanguage.Count == 0
            && httpContext.Request.Headers.TryGetValue(AcceptLanguageHeaderName, out var acceptLanguages)
            && acceptLanguages.Count > 0)
        {
            request.Headers.TryAddWithoutValidation(AcceptLanguageHeaderName, acceptLanguages.ToArray());
        }
    }

    private static Uri AppendCultureQuery(Uri requestUri, string cultureName)
    {
        var query = QueryHelpers.ParseNullableQuery(requestUri.Query);
        if (query is not null && query.ContainsKey(CultureQueryKey))
        {
            return requestUri;
        }

        var uriBuilder = new UriBuilder(requestUri);
        uriBuilder.Query = string.IsNullOrEmpty(uriBuilder.Query)
            ? $"{CultureQueryKey}={Uri.EscapeDataString(cultureName)}"
            : $"{uriBuilder.Query.TrimStart('?')}&{CultureQueryKey}={Uri.EscapeDataString(cultureName)}";
        return uriBuilder.Uri;
    }
}