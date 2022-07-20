﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Organizations;

public class DepartmentService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal DepartmentService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/department";
    }

    public async Task<DepartmentDetailDto> GetAsync(Guid id)
    {
        return await GetAsync<DepartmentDetailDto>($"Get?id={id}");
    }

    public async Task<List<DepartmentDto>> GetListAsync()
    {
        return await GetAsync<List<DepartmentDto>>($"List");
    }

    public async Task UpsertAsync(UpsertDepartmentDto upsertDepartmentDto)
    {
        await PostAsync("Save", upsertDepartmentDto);
    }

    public async Task CopyAsync(CopyDepartmentDto copyDepartmentDto)
    {
        await PostAsync("Copy", copyDepartmentDto);
    }

    public async Task RemoveAsync(Guid departmentId)
    {
        await DeleteAsync($"Remove?id={departmentId}");
    }

    public async Task<DepartmentChildrenCountDto> GetCountAsync()
    {
        return await GetAsync<DepartmentChildrenCountDto>($"Count");
    }
}
