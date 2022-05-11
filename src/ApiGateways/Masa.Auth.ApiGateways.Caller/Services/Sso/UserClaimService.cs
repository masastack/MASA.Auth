// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class UserClaimService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal UserClaimService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/sso/userClaim";
    }

    public async Task<PaginationDto<UserClaimDto>> GetListAsync(GetUserClaimsDto request)
    {
        return await SendAsync<GetUserClaimsDto, PaginationDto<UserClaimDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<UserClaimSelectDto>> GetSelectAsync(string search)
    {
        return await SendAsync<object, List<UserClaimSelectDto>>(nameof(GetSelectAsync), new { search });
    }

    public async Task<UserClaimDetailDto> GetDetailAsync(int id)
    {
        return await SendAsync<object, UserClaimDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task AddAsync(AddUserClaimDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateUserClaimDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(int id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveUserClaimDto(id));
    }
}

