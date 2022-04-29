// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class IdentityResourceClaim : UserClaim
{
    public int IdentityResourceId { get; private set; }

    public IdentityResource IdentityResource { get; private set; } = null!;
}

