// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class UpdateApiResourceDto : AddApiResourceDto
{
    public int Id { get; set; }

    public UpdateApiResourceDto()
    {
    }

    public UpdateApiResourceDto(int id, bool enabled, string name, string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable, List<int> apiScopes, List<int> userClaims, Dictionary<string, string> properties, List<string> secrets) : base(enabled, name, displayName, description, allowedAccessTokenSigningAlgorithms, showInDiscoveryDocument, lastAccessed, nonEditable, apiScopes, userClaims, properties, secrets)
    {
        Id = id;
    }
}

