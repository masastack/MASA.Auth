// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Permissions.Aggregates;

public class PermissionRelation : FullEntity<Guid, Guid>
{
    public Guid AffiliationPermissionId { get; private set; }

    public Guid LeadingPermissionId { get; private set; }

    public Permission AffiliationPermission { get; private set; } = null!;

    public Permission LeadingPermission { get; private set; } = null!;

    private PermissionRelation()
    {
    }

    public PermissionRelation(Guid leadingPermissionId, Guid affiliationPermissionId)
    {
        LeadingPermissionId = leadingPermissionId;
        AffiliationPermissionId = affiliationPermissionId;
    }
}

