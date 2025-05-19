// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Sso.Aggregates;

public class UserClaimExtend : Entity<int>
{
    /// <summary>
    /// User claim ID
    /// </summary>
    public Guid UserClaimId { get; private set; }

    /// <summary>
    /// Data source type
    /// </summary>
    public DataSourceTypes DataSourceType { get; private set; }

    /// <summary>
    /// Data source value
    /// When DataSourceType is FixedJson, stores a JSON string, e.g.: [{text:123,value:233}]
    /// When DataSourceType is Api, stores the API address
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
