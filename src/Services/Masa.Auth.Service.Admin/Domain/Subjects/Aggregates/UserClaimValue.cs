// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class UserClaimValue : AuditEntity<Guid, Guid>
{
    public User User { get; set; } = null!;

    public Guid UserId { get; set; }

    public string Name { get; init; }

    public string Value { get; private set; }

    public UserClaimValue(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public void UpdateValue(string value)
    {
        Value = value;
    }
}
