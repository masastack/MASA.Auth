namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class GlobalNavigationDialog
{
    [Parameter]
    public Guid UserId { get; set; }

    [Parameter]
    public GlobalNavigationModes GlobalNavigationMode { get; set; }

    [Parameter]
    public EventCallback<GlobalNavigation> OnSubmit { get; set; }

    public Stack.Components.Models.App App { get; set; } =new Stack.Components.Models.App("wuweilai","吴炜来");

    public string CategoryCode { get; set; } = "1234";

    public List<FavoriteNav>? FavoriteNavs { get; set; } = new ();
}

public enum GlobalNavigationModes
{
    User,
    Role,
    Navigation
}

public class GlobalNavigation
{
    public List<Guid> Roles { get; set; }

    public List<Permission> Permission { get; set; }

    public GlobalNavigation(List<Guid> roles, List<Permission> permission)
    {
        Roles = roles;
        Permission = permission;
    }
}

public class Permission
{
    public Guid PermissionId { get; set; }

    public bool Effect { get; set; }

    public Permission(Guid permissionId, bool effect)
    {
        PermissionId = permissionId;
        Effect = effect;
    }
}


