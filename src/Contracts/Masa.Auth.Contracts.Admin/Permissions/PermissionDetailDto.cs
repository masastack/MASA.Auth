// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class PermissionDetailDto : BaseUpsertDto<Guid>
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public PermissionTypes Type { get; set; }

    public string Description { get; set; } = string.Empty;

    public string SystemId { get; set; } = string.Empty;

    public string AppId { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool Enabled { get; set; } = true;

    public Guid ParentId { get; set; }

    public List<Guid> ApiPermissions { get; set; } = new();

    /// <summary>
    /// Used in special cases when auto-match left menu is highlighted
    /// </summary>
    public string MatchPattern { get; set; } = string.Empty;
}
