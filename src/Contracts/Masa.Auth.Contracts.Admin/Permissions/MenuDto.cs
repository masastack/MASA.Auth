// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class MenuDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public List<MenuDto> Children { get; set; } = new();
}
