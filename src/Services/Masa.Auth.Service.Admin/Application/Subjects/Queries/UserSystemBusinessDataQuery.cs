// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record UserSystemBusinessDataQuery(Guid UserId, string SystemId) : Query<string>
{
    public override string Result { get; set; } = string.Empty;
}
