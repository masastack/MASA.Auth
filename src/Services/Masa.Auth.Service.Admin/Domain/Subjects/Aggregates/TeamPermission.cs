// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class TeamPermission : SubjectPermissionRelation
{
    public TeamMemberTypes TeamMemberType { get; private set; }

    public TeamPermission(
        Guid permissionId, 
        bool effect, 
        TeamMemberTypes teamMemberType) 
        : base(permissionId, PermissionRelationTypes.TeamPermission, effect)
    {
        TeamMemberType = teamMemberType;
    }
}

