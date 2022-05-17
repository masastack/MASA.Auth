// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class TeamPermission : FullAuditEntity<Guid, Guid>
{
    private Team? _team;
    private Permission? _permission;

    public Team Team
    {
        get => _team ?? throw new UserFriendlyException("Failed to get team data");
        private set => _team = value;
    }

    public Permission Permission
    {
        get => _permission ?? throw new UserFriendlyException("Failed to get permission data");
        private set => _permission = value;
    }

    public Guid PermissionId { get; private set; }

    public bool Effect { get; private set; }

    public TeamMemberTypes TeamMemberType { get; private set; }

    public TeamPermission(Guid permissionId, bool effect, TeamMemberTypes teamMemberType)
    {
        PermissionId = permissionId;
        Effect = effect;
        TeamMemberType = teamMemberType;
    }
}

