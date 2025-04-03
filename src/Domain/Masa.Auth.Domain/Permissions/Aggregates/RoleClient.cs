// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Permissions.Aggregates;

public class RoleClient : ValueObject
{
    public Guid RoleId { get; private set; }

    public string ClientId { get; private set; } = default!;

    public RoleClient(Guid roleId, string clientId)
    {
        RoleId = roleId;
        ClientId = clientId;
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return RoleId;
        yield return ClientId;
    }
}
