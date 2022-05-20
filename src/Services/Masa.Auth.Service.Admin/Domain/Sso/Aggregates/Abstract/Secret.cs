// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

public abstract class Secret : FullAuditEntity<int, Guid>
{
    public string Description { get; protected set; } = string.Empty;

    public string Value { get; protected set; } = string.Empty;

    public DateTime? Expiration { get; protected set; }

    public string Type { get; protected set; } = "SharedSecret";
}
