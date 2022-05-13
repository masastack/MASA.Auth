// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class IdentityProvider : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    public string Name { get; protected set; } = null!;

    public string DisplayName { get; protected set; } = null!;

    public string Icon { get; protected set; } = null!;

    public IdentificationTypes IdentificationType { get; protected set; }
}
