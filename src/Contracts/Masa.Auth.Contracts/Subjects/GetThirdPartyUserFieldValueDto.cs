// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class GetThirdPartyUserFieldValueDto
{
    public string Scheme { get; set; } = default!;

    public string? Field { get; set; }

    public List<Guid> UserIds { get; set; } = new();
}
