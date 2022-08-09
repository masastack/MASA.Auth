// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions.ViewModels;

public class AppPermissionsViewModel
{
    public Guid Id { get; set; }

    public string AppId { get; set; } = string.Empty;

    public string AppTag { get; set; } = string.Empty;

    public string AppUrl { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsPermission { get; set; }

    public List<AppPermissionsViewModel> Children { get; set; } = new();
}
