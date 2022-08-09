// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Sso;

public class IdentityResourceService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal IdentityResourceService(ICaller callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/sso/identityResource";
    }

    public async Task<PaginationDto<IdentityResourceDto>> GetListAsync(GetIdentityResourcesDto request)
    {
        return await SendAsync<GetIdentityResourcesDto, PaginationDto<IdentityResourceDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<IdentityResourceSelectDto>> GetSelectAsync(string search = "")
    {
        return await SendAsync<object, List<IdentityResourceSelectDto>>(nameof(GetSelectAsync), new { search });
    }

    public async Task<IdentityResourceDetailDto> GetDetailAsync(int id)
    {
        return await SendAsync<object, IdentityResourceDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task AddAsync(AddIdentityResourceDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task AddStandardIdentityResourcesAsync()
    {
        await SendAsync<object?>(nameof(AddStandardIdentityResourcesAsync), null);
    }

    public async Task UpdateAsync(UpdateIdentityResourceDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(int id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveIdentityResourceDto(id));
    }
}

