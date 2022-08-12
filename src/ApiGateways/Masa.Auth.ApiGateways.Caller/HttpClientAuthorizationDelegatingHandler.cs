// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace Masa.Auth.ApiGateways.Caller;

public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
{
    readonly ITokenProvider _tokenProvider;
    readonly IHttpContextAccessor _httpContextAccessor;

    public HttpClientAuthorizationDelegatingHandler(ITokenProvider tokenProvider, IHttpContextAccessor httpContextAccessor)
    {
        _tokenProvider = tokenProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestProvider = _httpContextAccessor.HttpContext?.RequestServices.GetService<ITokenProvider>();
        if (!string.IsNullOrWhiteSpace(_tokenProvider.AccessToken))
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _tokenProvider.AccessToken);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
