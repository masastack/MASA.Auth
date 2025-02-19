// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class SaveI18NDisplayNamesInput
{
    public List<PermissionI18NDisplayNameDto> DisplayNames { get; set; } = new();

    public string Name { get; set; } = string.Empty;
}