// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Subjects.Aggregates;

public class UserRole : FullEntity<Guid, Guid>
{
    public User User { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public UserRole(Guid roleId)
    {
        RoleId = roleId;
    }

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
    }
}
