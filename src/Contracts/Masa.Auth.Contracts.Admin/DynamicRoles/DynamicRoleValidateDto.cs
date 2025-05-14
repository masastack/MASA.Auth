// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.DynamicRoles;

public class DynamicRoleValidateDto
{
    public Guid RoleId { get; set; }

    public bool IsValid { get; set; }

    public DynamicRoleValidateDto(Guid roleId, bool isValid)
    {
        RoleId = roleId;
        IsValid = isValid;
    }
}
