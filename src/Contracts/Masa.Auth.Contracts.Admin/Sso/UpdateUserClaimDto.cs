﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class UpdateUserClaimDto : AddUserClaimDto
{
    public Guid Id { get; set; }

    public UpdateUserClaimDto()
    {
    }

    public UpdateUserClaimDto(Guid id, string name, string description, UserClaimType userClaimType,
        DataSourceTypes dataSourceType = DataSourceTypes.None, string dataSourceValue = "")
        : base(name, description, userClaimType, dataSourceType, dataSourceValue)
    {
        Id = id;
    }
}

