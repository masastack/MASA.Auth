// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Projects.Queries;

public record ProjectListByAppIdsQuery : Query<List<ProjectDto>>
{
    public ProjectListByAppIdsQuery(IEnumerable<string>? appIds)
    {
        AppIds = appIds?
            .SelectMany(x => (x ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList() ?? new();
    }

    public List<string> AppIds { get; }

    public override List<ProjectDto> Result { get; set; } = new();
}