// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScopeProperty : Property
{
    public int ScopeId { get; private set; }

    public ApiScope Scope { get; private set; } = null!;
}
