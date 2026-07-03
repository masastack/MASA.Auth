// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ClientConfigService : ServiceBase
{
    public ClientConfigService() : base("api/sso/client-config")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };

        MapGet(GetDetailAsync);
        MapPost(UpsertAsync).RequireAuthorization();
    }

    private async Task<ClientConfigDto> GetDetailAsync(IEventBus eventBus, [FromQuery] string clientId)
    {
        var query = new ClientConfigDetailQuery(clientId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task UpsertAsync(IEventBus eventBus, [FromBody] ClientConfigDto clientConfigDto)
    {
        var command = new UpsertClientConfigCommand(clientConfigDto);
        await eventBus.PublishAsync(command);
    }
}
