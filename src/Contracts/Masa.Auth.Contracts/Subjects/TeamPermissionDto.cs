// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects
{
    public class TeamPermissionDto : SubjectPermissionRelationDto
    {
        public TeamMemberTypes TeamMemberType { get; set; }

        public TeamPermissionDto(Guid permissionId, bool effect, TeamMemberTypes teamMemberType) : base(permissionId, effect)
        {
            TeamMemberType = teamMemberType;
        }
    }
}
