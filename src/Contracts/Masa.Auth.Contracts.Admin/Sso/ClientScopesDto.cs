// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientScopesDto
{
    public List<CheckItemDto<int>> IdentityScopes { get; set; } = new();

    public List<CheckItemDto<int>> ApiScopes { get; set; } = new();

    public List<string> AllowedScopes
    {
        get
        {
            return IdentityScopes.Union(ApiScopes).Where(s => s.Selected).Select(s => s.DisplayValue).ToList();
        }
    }
}
