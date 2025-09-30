// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Aggregates;

public class ResourceIdentifier : ValueObject
{
    public string Service { get; set; } = "*";

    public string Region { get; set; } = "*";

    public string Identifier { get; set; } = "*";

    /// <summary>
    /// EF Core 无参构造函数
    /// </summary>
    public ResourceIdentifier()
    {
    }

    /// <summary>
    /// 从字符串解析Resource，格式为 Service:Region:Identifier
    /// </summary>
    /// <param name="resource">资源标识符字符串</param>
    public ResourceIdentifier(string? resource)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            return; // 使用默认值 "*"
        }

        var parts = resource.Split(':', StringSplitOptions.None);

        if (parts.Length >= 1 && !string.IsNullOrEmpty(parts[0]))
            Service = parts[0];

        if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[1]))
            Region = parts[1];

        if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
            Identifier = parts[2];
    }

    public override string ToString()
    {
        return $"{Service}:{Region}:{Identifier}";
    }

    protected override IEnumerable<object> GetEqualityValues()
    {
        yield return Service;
        yield return Region;
        yield return Identifier;
    }
}
