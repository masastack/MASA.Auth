﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Sso;

public class UserClaimService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal UserClaimService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/sso/userClaim";
    }

    public async Task<PaginationDto<UserClaimDto>> GetListAsync(GetUserClaimsDto request)
    {
        return await SendAsync<GetUserClaimsDto, PaginationDto<UserClaimDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<UserClaimSelectDto>> GetSelectAsync(string? search = null)
    {
        return await SendAsync<object, List<UserClaimSelectDto>>(nameof(GetSelectAsync), new { search });
    }

    public async Task<UserClaimDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, UserClaimDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task AddAsync(AddUserClaimDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task AddStandardUserClaimsAsync()
    {
        await SendAsync<object?>(nameof(AddStandardUserClaimsAsync), null);
    }

    public async Task UpdateAsync(UpdateUserClaimDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveUserClaimDto(id));
    }
}

