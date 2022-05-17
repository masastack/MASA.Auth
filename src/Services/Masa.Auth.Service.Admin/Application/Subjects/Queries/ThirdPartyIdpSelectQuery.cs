// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record ThirdPartyIdpSelectQuery(string? Search) : Query<List<ThirdPartyIdpSelectDto>>
{
    public override List<ThirdPartyIdpSelectDto> Result { get; set; } = new();
}
