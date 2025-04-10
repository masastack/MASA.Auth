// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Aggregates;

public class DynamicRole : FullAggregateRoot<Guid, Guid>
{
    public string ClientId { get; private set; } = default!;

    public string Name { get; private set; } = default!;

    public string Code { get; private set; } = default!;

    public bool Enabled { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public List<DynamicRuleCondition> Conditions { get; private set; } = new();

    private DynamicRole() { }

    public DynamicRole(string clientId, string name, string code, bool enabled, string description, List<DynamicRuleCondition> conditions)
    {
        ClientId = clientId;
        Name = name;
        Code = code;
        Enabled = enabled;
        Description = description;
        Conditions = conditions;
    }
}