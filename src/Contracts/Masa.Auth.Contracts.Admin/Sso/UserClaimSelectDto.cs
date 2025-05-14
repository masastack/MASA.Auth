// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class UserClaimSelectDto : UserClaimDto
{
    public DataSourceTypes DataSourceType { get; set; }

    public string DataSourceValue { get; set; } = "";

    public UserClaimSelectDto(Guid id, string name, string description) : base(id, name, description, default)
    {
    }
}

