// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientScopesDto
{
    public List<CheckItemDto<string>> IdentityResources { get; set; } = new();

    public List<CheckItemDto<string>> ApiScopes { get; set; } = new();

    public List<string> _AllowedScopes { get; private set; } = ClientConsts.MandatoryResource.ToList();

    public List<string> AllowedScopes
    {
        get
        {
            var allScopes = IdentityResources.Union(ApiScopes);
            var allowedScopes = _AllowedScopes.Where(x => allScopes.Any(s => s.Selected && s.Id == x)).ToList();
            if (allowedScopes.Any())
            {
                return allScopes.Where(s => s.Selected).Select(s => s.Id).Union(allowedScopes).ToList();
            }
            return _AllowedScopes;
        }
        set
        {
            _AllowedScopes = value;
        }
    }
}
