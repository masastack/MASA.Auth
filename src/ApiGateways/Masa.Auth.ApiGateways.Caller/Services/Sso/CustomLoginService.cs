// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class CustomLoginService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal CustomLoginService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/sso/customLogin";
    }

    public async Task<PaginationDto<CustomLoginDto>> GetListAsync(GetCustomLoginsDto request)
    {
        return await SendAsync<GetCustomLoginsDto, PaginationDto<CustomLoginDto>>(nameof(GetListAsync), request);
    }

    public async Task<CustomLoginDetailDto> GetDetailAsync(int id)
    {
        return await SendAsync<object, CustomLoginDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task AddAsync(AddCustomLoginDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateCustomLoginDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(int id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveCustomLoginDto(id));
    }
}

