// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Aggregates;

public class DynamicRuleCondition : ValueObject
{
    public LogicalOperator LogicalOperator { get; private set; }

    public string FieldName { get; private set; } = default!;

    public OperatorType OperatorType { get; private set; }

    public string Value { get; private set; } = string.Empty;

    public DynamicRoleDataType DataType { get; private set; }

    public DynamicRuleCondition(LogicalOperator logicalOperator, string fieldName, OperatorType operatorType, string value, DynamicRoleDataType dataType)
    {
        LogicalOperator = logicalOperator;
        FieldName = fieldName;
        OperatorType = operatorType;
        Value = value;
        DataType = dataType;
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return LogicalOperator;
        yield return FieldName;
        yield return OperatorType;
        yield return DataType;
    }
}