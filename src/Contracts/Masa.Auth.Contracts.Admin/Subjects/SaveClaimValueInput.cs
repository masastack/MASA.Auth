// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class SaveClaimValueInput
{
    public Guid UserId { get; set; }

    public string ClaimName { get; set; } = default!;

     public string ClaimValue { get; set; } = string.Empty;
}
