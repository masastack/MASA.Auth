// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class UpdateApiScopeDto : AddApiScopeDto
{
    public int Id { get; set; }

    public UpdateApiScopeDto()
    {
    }

    public UpdateApiScopeDto(int id, bool enabled, string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, List<int> userClaims, Dictionary<string, string> properties) : base(enabled, name, displayName, description, required, emphasize, showInDiscoveryDocument, userClaims, properties)
    {
        Id = id;
    }
}

