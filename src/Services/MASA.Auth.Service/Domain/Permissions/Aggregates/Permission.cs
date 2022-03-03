namespace MASA.Auth.Service.Domain.Permissions.Aggregates;

public class Permission : AuditAggregateRoot<Guid, Guid>
{
    public string AppId { get; private set; } = "";

    public string Name { get; private set; } = "";

    public string Code { get; private set; } = "";

    public string Url { get; private set; } = "";

    public string Icon { get; private set; } = "";

    public PermissionType Type { get; private set; }

    public string Description { get; private set; } = "";

    public bool Enabled { get; private set; }

    private List<PermissionItem> permissionItems = new();

    public IReadOnlyCollection<PermissionItem> PermissionItems => permissionItems;

    private List<RolePermission> rolePermissions = new();

    public IReadOnlyCollection<RolePermission> RolePermissions => rolePermissions;
}

public enum PermissionType
{
    Menu,
    Element,
    Api,
    Data
}