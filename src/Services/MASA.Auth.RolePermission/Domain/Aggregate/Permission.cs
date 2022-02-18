namespace MASA.Auth.RolePermission.Domain.Aggregate;

public class Permission : Entity<int>
{
    public string Name { get; set; } = "";

    public string Code { get; set; } = "";

    public string Description { get; set; } = "";

    public State State { get; set; }

    public string Url { get; set; } = "";

    public string Icon { get; set; } = "";

    public PermissionType Type { get; set; }

    public int? ParentId { get; set; }

    public Permission? ParentPermission { get; set; }

    private List<PermissionApiItem> permissionApiItems = new();

    public IReadOnlyCollection<PermissionApiItem> PermissionApiItems => permissionApiItems;

}

