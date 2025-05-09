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

    public bool EvaluateConditions(User user)
    {
        if (Conditions == null || !Conditions.Any()) return true; // No conditions means always true.

        var result = false;
        foreach (var condition in Conditions.OrderBy(c => c.Order))
        {
            var isConditionMet = condition.EvaluateCondition(user);
            if (condition.LogicalOperator == LogicalOperator.And)
            {
                result = result && isConditionMet;
            }
            else if (condition.LogicalOperator == LogicalOperator.Or)
            {
                result = result || isConditionMet;
            }
            else
            {
                result = isConditionMet; // Default to first condition's result if no operator specified.
            }

            if (!result && condition.LogicalOperator == LogicalOperator.And)
            {
                break; // If using AND and one condition fails, overall result is false.
            }
        }
        return result;
    }
}