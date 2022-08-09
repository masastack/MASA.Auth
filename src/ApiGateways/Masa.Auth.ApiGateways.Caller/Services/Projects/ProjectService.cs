// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Projects;

public class ProjectService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ProjectService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/project";
    }

    public async Task<List<ProjectDto>> GetListAsync(bool hasMenu = false)
    {
        return await GetAsync<List<ProjectDto>>($"GetList?hasMenu={hasMenu}");
    }

    public async Task<List<string>> GetAppTagsAsync()
    {
        return await GetAsync<List<string>>($"GetTags");
    }

    public async Task SaveAppTagAsync(AppTagDetailDto dto)
    {
        await PostAsync("SaveAppTag", dto);
    }
}
