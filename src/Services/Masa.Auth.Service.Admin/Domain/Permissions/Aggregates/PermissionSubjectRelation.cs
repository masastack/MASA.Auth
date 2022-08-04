// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates
{
    public class PermissionSubjectRelation : FullEntity<Guid, Guid>
    {
        public Guid PermissionId { get; set; }

        public PermissionRelationTypes PermissionRelationType { get; private set; }

        public Guid SubjectRelationId { get; private set; }

        public bool Effect { get; private set; }

        public Permission Permission { get; private set; } = default!;

        public Role Role { get; private set; } = default!;

        //public User User { get; private set; } = default!;

        //public Team Team { get; private set; } = default!;

        public PermissionSubjectRelation(Guid permissionId, PermissionRelationTypes permissionRelationType, bool effect)
        {
            PermissionId = permissionId;
            PermissionRelationType = permissionRelationType;
            Effect = effect;
        }

        public void Update(bool effect)
        {
            Effect = effect;
        }

        public static implicit operator PermissionSubjectRelationDto(PermissionSubjectRelation psr)
        {
            return new(psr.PermissionId, psr.Effect);
        }
    }
}
