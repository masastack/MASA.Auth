// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class AddUserClaimDto
{
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public UserClaimType UserClaimType { get; set; }

    /// <summary>
    /// Data source type
    /// </summary>
    public DataSourceTypes DataSourceType { get; set; } = DataSourceTypes.None;

    /// <summary>
    /// Data source value
    /// When DataSourceType is FixedJson, stores a JSON string, e.g.: [{key:123,value:233}]
    /// When DataSourceType is Api, stores the API address
    /// </summary>
    public string DataSourceValue { get; set; } = "";

    public AddUserClaimDto()
    {
    }

    public AddUserClaimDto(string name, string description, UserClaimType userClaimType,
        DataSourceTypes dataSourceType = DataSourceTypes.None, string dataSourceValue = "")
    {
        Name = name;
        Description = description;
        UserClaimType = userClaimType;
        DataSourceType = dataSourceType;
        DataSourceValue = dataSourceValue;
    }
}

