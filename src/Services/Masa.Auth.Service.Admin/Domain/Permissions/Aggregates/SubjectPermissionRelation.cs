// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates
{
    public class SubjectPermissionRelation : FullEntity<Guid, Guid>
    {
        public Guid PermissionId { get; protected set; }

        public PermissionRelationTypes PermissionRelationType { get; protected set; }

        public Guid SubjectRelationId { get; protected set; }

        public bool Effect { get; protected set; }

        public Permission Permission { get; protected set; } = default!;

        public Role Role { get; protected set; } = default!;

        public User User { get; protected set; } = default!;

        public Team Team { get; protected set; } = default!;

        public SubjectPermissionRelation(Guid permissionId, PermissionRelationTypes permissionRelationType, bool effect)
        {
            PermissionId = permissionId;
            PermissionRelationType = permissionRelationType;
            Effect = effect;
        }

        public void Update(bool effect)
        {
            Effect = effect;
        }

        public static implicit operator SubjectPermissionRelationDto(SubjectPermissionRelation spr)
        {
            return new(spr.PermissionId, spr.Effect);
        }
    }
}
