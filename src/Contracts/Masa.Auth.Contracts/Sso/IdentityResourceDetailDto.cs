// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class IdentityResourceDetailDto : IdentityResourceDto
{
    public List<Guid> UserClaims { get; set; }

    public Dictionary<string, string> Properties { get; set; }

    public IdentityResourceDetailDto()
    {
        UserClaims = new();
        Properties = new();
    }

    public IdentityResourceDetailDto(Guid id, string name, string displayName, string description, bool enabled, bool required, bool emphasize, bool showInDiscoveryDocument, bool nonEditable, List<Guid> userClaims, Dictionary<string, string> properties) : base(id, name, displayName, description, enabled, required, emphasize, showInDiscoveryDocument, nonEditable)
    {
        UserClaims = userClaims;
        Properties = properties;
    }

    public static implicit operator UpdateIdentityResourceDto(IdentityResourceDetailDto idrs)
    {
        return new UpdateIdentityResourceDto(idrs.Id, idrs.DisplayName, idrs.Description, idrs.Enabled, idrs.Required, idrs.Emphasize, idrs.ShowInDiscoveryDocument, idrs.NonEditable, idrs.UserClaims, idrs.Properties);
    }
}

