// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.


namespace Masa.Auth.Domain.DynamicRoles.Aggregates;

public class ActionIdentifier : ValueObject
{
    public string Resource { get; set; } = "*";

    public string Type { get; set; } = "*";

    public string Operation { get; set; } = "*";

    /// <summary>
    /// EF Core 无参构造函数
    /// </summary>
    public ActionIdentifier()
    {
    }

    /// <summary>
    /// 从字符串解析ActionIdentifier，格式为 Resource:Type:Operation
    /// </summary>
    /// <param name="actionName">操作标识符字符串</param>
    public ActionIdentifier(string? actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            return; // 使用默认值 "*"
        }

        var parts = actionName.Split(':', StringSplitOptions.None);

        if (parts.Length >= 1 && !string.IsNullOrEmpty(parts[0]))
            Resource = parts[0];

        if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[1]))
            Type = parts[1];

        if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
            Operation = parts[2];
    }

    public override string ToString()
    {
        return $"{Resource}:{Type}:{Operation}";
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Resource;
        yield return Type;
        yield return Operation;
    }
}

