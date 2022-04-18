namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Permissions.ViewModels;

internal class AppPermissionsViewModel
{
    public Guid Id { get; set; }

    public string AppId { get; set; } = string.Empty;

    public string AppTag { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool IsPermission { get; set; }

    public List<AppPermissionsViewModel> Children { get; set; } = new();
}
