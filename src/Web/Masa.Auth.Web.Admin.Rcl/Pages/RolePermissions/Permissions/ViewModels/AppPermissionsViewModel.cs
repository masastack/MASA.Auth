// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions.ViewModels;

public class AppPermissionsViewModel : ICloneable
{
    public AppPermissionsViewModel()
    {
    }

    public AppPermissionsViewModel(Guid id, string appId, string appTag, string appUrl, string name, bool isPermission, PermissionTypes? type, List<AppPermissionsViewModel> children)
    {
        Id = id;
        AppId = appId;
        AppTag = appTag;
        AppUrl = appUrl;
        Name = name;
        IsPermission = isPermission;
        Type = type;
        Children = children;
    }

    public Guid Id { get; set; }

    public string AppId { get; set; } = string.Empty;

    public string AppTag { get; set; } = string.Empty;

    public string AppUrl { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsPermission { get; set; }

    public PermissionTypes? Type { get; set; }

    public List<AppPermissionsViewModel> Children { get; set; } = new();

    public GlobalNavVisibleTypes? VisibleType { get; set; }

    public object Clone()
    {
        return new AppPermissionsViewModel(Id, AppId, AppTag, AppUrl, Name, IsPermission,
            Type, Children.Select(c => (AppPermissionsViewModel)c.Clone()).ToList());
    }
}
