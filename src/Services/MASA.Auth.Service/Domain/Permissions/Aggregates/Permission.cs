namespace Masa.Auth.Service.Domain.Permissions.Aggregates;

public class Permission : AuditAggregateRoot<Guid, Guid>
{
    public int SystemId { get; set; }

    public string AppId { get; private set; } = "";

    public string Name { get; private set; } = "";

    public string Code { get; private set; } = "";

    public Guid ParentId { get; set; }

    public string Url { get; private set; } = "";

    public string Icon { get; private set; } = "";

    public PermissionType Type { get; private set; }

    public string Description { get; private set; } = "";

    public bool Enabled { get; private set; }

    private List<PermissionRelation> permissionItems = new();

    public IReadOnlyCollection<PermissionRelation> PermissionItems => permissionItems;

    private List<RolePermission> rolePermissions = new();

    public IReadOnlyCollection<RolePermission> RolePermissions => rolePermissions;

    private List<UserPermission> userPermissions = new();

    public IReadOnlyCollection<UserPermission> UserPermissions => userPermissions;

    private List<TeamPermission> teamPermissions = new();

    public IReadOnlyCollection<TeamPermission> TeamPermissions => teamPermissions;

    public void DeleteCheck()
    {
        if (rolePermissions.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,because RolePermissions not empty!");
        }
        if (userPermissions.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,because UserPermissions not empty!");
        }
        if (permissionItems.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,because PermissionItems not empty!");
        }
    }
}

public enum PermissionType
{
    Menu,
    Element,
    Api,
    Data
}