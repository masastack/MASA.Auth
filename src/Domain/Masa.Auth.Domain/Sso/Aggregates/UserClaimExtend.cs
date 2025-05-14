// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Sso.Aggregates;

public class UserClaimExtend : Entity<int>
{
    /// <summary>
    /// 用户声明ID
    /// </summary>
    public Guid UserClaimId { get; private set; }

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceTypes DataSourceType { get; private set; }

    /// <summary>
    /// 数据源值
    /// 当DataSourceType为FixedJson时，存储JSON字符串，如：[{text:123,value:233}]
    /// 当DataSourceType为Api时，存储API地址
    /// </summary>
    public string DataSourceValue { get; private set; } = "";

    public string ItemText { get; set; } = "text";

    public string ItemValue { get; set; } = "value";

    private UserClaimExtend() { }

    public UserClaimExtend(Guid userClaimId, DataSourceTypes dataSourceType, string dataSourceValue)
    {
        UserClaimId = userClaimId;
        DataSourceType = dataSourceType;
        DataSourceValue = dataSourceValue;
    }

    public void UpdateDataSource(DataSourceTypes dataSourceType, string dataSourceValue)
    {
        DataSourceType = dataSourceType;
        DataSourceValue = dataSourceValue;
    }
}
