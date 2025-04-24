// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.DynamicRoles;

public class DynamicRuleConditionDto
{
    public LogicalOperator LogicalOperator { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public OperatorTypes OperatorType { get; set; }

    public string Value { get; set; } = string.Empty;

    public DynamicRoleDataTypes DataType { get; set; }

    public int Order { get; set; }
}
