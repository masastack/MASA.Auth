// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Organizations;

public class PositionService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal PositionService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/position/";
    }

    public async Task<PaginationDto<PositionDto>> GetListAsync(GetPositionsDto request)
    {
        return await SendAsync<GetPositionsDto, PaginationDto<PositionDto>>(nameof(GetListAsync), request);
    }

    public async Task<PositionDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, PositionDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task<List<PositionSelectDto>> GetSelectAsync(string name = "")
    {
        return await SendAsync<object, List<PositionSelectDto>>(nameof(GetSelectAsync), new { name });
    }

    public async Task AddAsync(AddPositionDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdatePositionDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await SendAsync(nameof(RemoveAsync), new RemovePositionDto(id));
    }
}

