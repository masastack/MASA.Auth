// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResourceScope : Entity<int>
{
    public int ApiScopeId { get; private set; }

    public ApiScope ApiScope { get; private set; } = null!;

    public int ApiResourceId { get; private set; }

    public ApiResource ApiResource { get; private set; } = null!;

    public ApiResourceScope(int apiScopeId)
    {
        ApiScopeId = apiScopeId;
    }
}

