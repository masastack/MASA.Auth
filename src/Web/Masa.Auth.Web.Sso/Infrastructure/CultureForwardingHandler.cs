// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.WebUtilities;

namespace Masa.Auth.Web.Sso.Infrastructure;

/// <summary>
/// 将入站请求的语言信息透传给下游服务（当前为 Auth 服务），
/// 使下游返回的 UserFriendlyException 报错文案按请求语言本地化。
/// 语言来源与 <see cref="RequestLocalizationMiddleware"/> 一致，优先级：
/// URL ?culture= > Accept-Language > 默认语言。
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

        // 1) 优先使用 RequestLocalization（UseI18n 中间件）解析出的请求文化，
        //    此时无需关心语言来自 query、Cookie 还是 Accept-Language
        var resolvedCulture = httpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.Culture;
        if (resolvedCulture is not null && !string.IsNullOrEmpty(resolvedCulture.Name))
        {
            request.RequestUri = AppendCultureQuery(request.RequestUri, resolvedCulture.Name);
            return;
        }

        // 2) 兜底：UseI18n 未生效时（如后台线程调用），原样转发语言信息，
        //    由下游服务端按相同优先级自行解析
        if (httpContext.Request.Query.TryGetValue(CultureQueryKey, out var cultureQuery) && !string.IsNullOrEmpty(cultureQuery))
        {
            request.RequestUri = AppendCultureQuery(request.RequestUri, cultureQuery.ToString());
        }

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