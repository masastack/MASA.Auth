// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record AppElementPermissionCodeListQuery(string AppId, Guid UserId) : Query<List<string>>
{
    public override List<string> Result { get; set; } = new();
}
