// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientScopesDto
{
    public List<CheckItemDto<string>> IdentityScopes { get; set; } = new();

    public List<CheckItemDto<string>> ApiScopes { get; set; } = new();

    public List<string> _AllowedScopes { get; private set; } = ClientConsts.MandatoryResource.ToList();

    public List<string> AllowedScopes
    {
        get
        {
            return IdentityScopes.Union(ApiScopes).Where(s => s.Selected)
                .Select(s => s.Id).Union(_AllowedScopes).ToList();
        }
        set
        {
            _AllowedScopes = value;
        }
    }
}
