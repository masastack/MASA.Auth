// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public class HttpClientEnvironmentDelegatingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpClientEnvironmentDelegatingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var userClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == IsolationConsts.ENVIRONMENT_KEY);
        if (userClaim != null)
        {
            _httpContextAccessor.HttpContext?.Items.Add(IsolationConsts.ENVIRONMENT_KEY, userClaim.Value);
            request.Headers.Add(IsolationConsts.ENVIRONMENT_KEY, userClaim.Value);
        }
        return await base.SendAsync(request, cancellationToken);
    }
}
