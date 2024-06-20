// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Projects.Queries;

public record NavigationListQuery(Guid UserId, string ClientId) : Query<List<ProjectDto>>
{
    public override List<ProjectDto> Result { get; set; } = new();
}
