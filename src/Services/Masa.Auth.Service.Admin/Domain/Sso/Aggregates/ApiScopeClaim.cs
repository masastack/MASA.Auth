// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScopeClaim : Entity<int>
{
    public int UserClaimId { get; private set; }

    public UserClaim UserClaim { get; private set; } = null!;

    public int ApiScopeId { get; private set; }

    public ApiScope ApiScope { get; private set; } = null!;

    public ApiScopeClaim(int userClaimId)
    {
        UserClaimId = userClaimId;
    }
}
