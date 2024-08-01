// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record ThirdPartyUserFieldValueQuery(string Scheme, List<Guid> UserIds, string? Field) : Query<Dictionary<Guid, string>>
{
    public override Dictionary<Guid, string> Result { get; set; } = new();
}