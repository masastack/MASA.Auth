// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.DynamicRoles;

public class DynamicRoleUpsertDto
{
    public string ClientId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public bool Enabled { get; set; }

    public string Description { get; set; } = string.Empty;

    public DateTime CreationTime { get; set; }

    public DateTime ModificationTime { get; set; }

    public List<DynamicRuleConditionDto> Conditions { get; set; } = new();
}
