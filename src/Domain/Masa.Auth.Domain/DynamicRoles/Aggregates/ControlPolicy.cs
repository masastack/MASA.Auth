// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.DynamicRoles.Aggregates;

public class ControlPolicy : Entity<Guid>
{
    /// <summary>
    /// 策略名称，便于识别和管理
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 策略效果：Allow 或 Deny
    /// </summary>
    public StatementEffect Effect { get; set; } = StatementEffect.Deny;

    /// <summary>
    /// 策略优先级，数值越大优先级越高
    /// </summary>
    public int Priority { get; set; } = 0;

    /// <summary>
    /// 是否启用此策略
    /// </summary>
    public bool Enabled { get; set; } = true;

    public IList<ActionIdentifier> Actions { get; set; } = new List<ActionIdentifier>();

    public IList<ResourceIdentifier> Resources { get; set; } = new List<ResourceIdentifier>();
}