// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.CacheModels;

public class CachePermission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public PermissionTypes Type { get; set; } = PermissionTypes.Api;

    public string Description { get; set; } = string.Empty;

    public string SystemId { get; set; } = string.Empty;

    public string AppId { get; set; } = string.Empty;

    public int Order { get; set; }

    public string Url { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string MatchPattern { get; set; } = string.Empty;

    public Guid ParentId { get; set; }

    public bool Enabled { get; set; }

    public List<Guid> ApiPermissions { get; set; } = new();
}
