﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class UserClaimService : RestServiceBase
{
    public UserClaimService() : base("api/sso/userClaim")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
    }

    private async Task<PaginationDto<UserClaimDto>> GetListAsync(IEventBus eventBus, GetUserClaimsDto userClaim)
    {
        var query = new UserClaimsQuery(userClaim.Page, userClaim.PageSize, userClaim.Search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<UserClaimDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new UserClaimDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<UserClaimSelectDto>> GetSelectAsync([FromServices] IEventBus eventBus, [FromQuery] string? search)
    {
        var query = new UserClaimSelectQuery(search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddAsync(IEventBus eventBus, [FromBody] AddUserClaimDto dto)
    {
        await eventBus.PublishAsync(new AddUserClaimCommand(dto));
    }

    private async Task AddStandardUserClaimsAsync(IEventBus eventBus)
    {
        await eventBus.PublishAsync(new AddStandardUserClaimsCommand());
    }

    private async Task UpdateAsync(
        IEventBus eventBus,
        [FromBody] UpdateUserClaimDto dto)
    {
        await eventBus.PublishAsync(new UpdateUserClaimCommand(dto));
    }

    private async Task RemoveAsync(
        IEventBus eventBus,
        [FromBody] RemoveUserClaimDto dto)
    {
        await eventBus.PublishAsync(new RemoveUserClaimCommand(dto));
    }
}
