// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ThirdPartyUserService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ThirdPartyUserService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/thirdPartyUser/";
    }

    public async Task<PaginationDto<ThirdPartyUserDto>> GetListAsync(GetThirdPartyUsersDto request)
    {
        return await SendAsync<GetThirdPartyUsersDto, PaginationDto<ThirdPartyUserDto>>(nameof(GetListAsync), request);
    }

    public async Task<ThirdPartyUserDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, ThirdPartyUserDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task RemoveAsync(Guid id)
    {
        await DeleteAsync(nameof(RemoveAsync), new { id });
    }

    public async Task RemoveByThridPartyIdentityAsync(string thridPartyIdentity)
    {
        await DeleteAsync(nameof(RemoveByThridPartyIdentityAsync), new { thridPartyIdentity });
    }
}

