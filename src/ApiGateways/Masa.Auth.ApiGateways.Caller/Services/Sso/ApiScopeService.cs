// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ApiScopeService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ApiScopeService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/sso/apiScope";
    }

    public async Task<PaginationDto<ApiScopeDto>> GetListAsync(GetApiScopesDto request)
    {
        return await SendAsync<GetApiScopesDto, PaginationDto<ApiScopeDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<ApiScopeSelectDto>> GetSelectAsync(string search)
    {
        return await SendAsync<object, List<ApiScopeSelectDto>>(nameof(GetSelectAsync), new { search });
    }

    public async Task<ApiScopeDetailDto> GetDetailAsync(int id)
    {
        return await SendAsync<object, ApiScopeDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task AddAsync(AddApiScopeDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateApiScopeDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(int id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveApiScopeDto(id));
    }
}

