// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class UserClaimDetailDto : UserClaimDto
{
    public UserClaimDetailDto() { }

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceTypes DataSourceType { get; set; } = DataSourceTypes.None;

    /// <summary>
    /// 数据源值
    /// 当DataSourceType为FixedJson时，存储JSON字符串，如：[{key:123,value:233}]
    /// 当DataSourceType为Api时，存储API地址
    /// </summary>
    public string DataSourceValue { get; set; } = "";

    public UserClaimDetailDto(Guid id, string name, string description, UserClaimType userClaimType,
        DataSourceTypes dataSourceType = DataSourceTypes.None, string dataSourceValue = "")
        : base(id, name, description, userClaimType)
    {
        DataSourceType = dataSourceType;
        DataSourceValue = dataSourceValue;
    }

    public static implicit operator UpdateUserClaimDto(UserClaimDetailDto userClaim)
    {
        return new UpdateUserClaimDto(userClaim.Id, userClaim.Name, userClaim.Description, userClaim.UserClaimType,
            userClaim.DataSourceType, userClaim.DataSourceValue);
    }
}

