// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Service.Caller;

namespace Masa.Auth.ApiGateways.Caller.Services.Sso;

public class ClientService : ServiceBase
{
    protected override string BaseUrl { get; set; } = "";

    public ClientService(ICaller callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/sso/client";
    }

    public async Task<PaginationDto<ClientDto>> GetListAsync(GetClientPaginationDto clientPaginationDto)
    {
        return await GetAsync<GetClientPaginationDto, PaginationDto<ClientDto>>("GetList", clientPaginationDto);
    }

    public async Task<ClientDetailDto> GetDetailAsync(int id)
    {
        return await GetAsync<ClientDetailDto>($"{nameof(GetDetailAsync)}?id={id}");
    }

    public async Task<List<ClientTypeDetailDto>> GetClientTypeListAsync()
    {
        return await GetAsync<List<ClientTypeDetailDto>>("GetClientTypeList");
    }

    public async Task<List<ClientSelectDto>> GetClientSelectAsync()
    {
        return await GetAsync<List<ClientSelectDto>>(nameof(GetClientSelectAsync));
    }

    public async Task AddClientAsync(AddClientDto addClientDto)
    {
        await PostAsync(nameof(AddClientAsync), addClientDto);
    }

    public async Task UpdateClientAsync(ClientDetailDto clientDetailDto)
    {
        await PostAsync(nameof(UpdateClientAsync), clientDetailDto);
    }

    public async Task RemoveClientAsync(int id)
    {
        await DeleteAsync($"{nameof(RemoveClientAsync)}?id={id}");
    }
}
