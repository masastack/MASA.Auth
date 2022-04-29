// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Projects.Queries;

public record AppTagsQuery : Query<List<string>>
{
    public override List<string> Result { get; set; } = new();
}
