// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ApiResourceService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ApiResourceService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/sso/apiResource";
    }

    public async Task<PaginationDto<ApiResourceDto>> GetListAsync(GetApiResourcesDto request)
    {
        return await SendAsync<GetApiResourcesDto, PaginationDto<ApiResourceDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<ApiResourceSelectDto>> GetSelectAsync(string? search = null)
    {
        return await SendAsync<object, List<ApiResourceSelectDto>>(nameof(GetSelectAsync), new { search });
    }

    public async Task<ApiResourceDetailDto> GetDetailAsync(int id)
    {
        return await SendAsync<object, ApiResourceDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task AddAsync(AddApiResourceDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateApiResourceDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(int id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveApiResourceDto(id));
    }
}

