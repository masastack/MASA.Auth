// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class EndpointRowDataProvider : ISingletonDependency
{
    List<string?> _endpoints = new();

    public List<string?> Endpoints => _endpoints;

    public EndpointRowDataProvider(IEnumerable<EndpointDataSource> endpointSources)
    {
        _endpoints = endpointSources.SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.DisplayName != null && endpoint.DisplayName.Contains("=>"))
            .Select(endpoint => endpoint.RoutePattern.RawText).ToList();
    }
}
