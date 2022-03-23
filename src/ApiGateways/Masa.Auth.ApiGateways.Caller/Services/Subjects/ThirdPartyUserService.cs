﻿namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ThirdPartyUserService : ServiceBase
{
    internal ThirdPartyUserService(ICallerProvider callerProvider) : base(callerProvider)
    {
    }

    public async Task<PaginationDto<ThirdPartyUserDto>> GetThirdPartyUsersAsync(GetThirdPartyUsersDto request)
    {
        return await Task.FromResult(new PaginationDto<ThirdPartyUserDto>(0, 0, new List<ThirdPartyUserDto>()));
    }

    public async Task<ThirdPartyUserDetailDto> GetThirdPartyUserDetailAsync(Guid id)
    {
        return await Task.FromResult(new ThirdPartyUserDetailDto());
    }

    public async Task AddThirdPartyUserAsync(AddThirdPartyUserDto request)
    {
        await Task.CompletedTask;
    }
}

