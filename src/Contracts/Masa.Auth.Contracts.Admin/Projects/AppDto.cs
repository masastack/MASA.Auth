// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Projects;

public class AppDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Identity { get; set; } = string.Empty;

    public string Tag { get; set; } = string.Empty;

    public AppTypes Type { get; set; }

    public int ProjectId { get; set; }

    public string Url { get; set; } = string.Empty;

    public List<PermissionNavDto> Navs { get; set; } = new();
}
