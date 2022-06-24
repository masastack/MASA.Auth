// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public class HttpClientEnvironmentDelegatingHandler : DelegatingHandler
{
    public HttpClientEnvironmentDelegatingHandler()
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //request.Headers.Add(IsolationConsts.ENVIRONMENT_KEY, "development");
        return await base.SendAsync(request, cancellationToken);
    }
}
