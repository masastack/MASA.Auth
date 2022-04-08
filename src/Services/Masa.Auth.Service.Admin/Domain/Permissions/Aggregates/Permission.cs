namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class Permission : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    public int SystemId { get; set; }

    public string AppId { get; private set; }

    public string Name { get; private set; }

    public string Code { get; private set; }

    public Guid ParentId { get; set; }

    public string Url { get; private set; } = "";

    public string Icon { get; private set; } = "";

    public PermissionTypes Type { get; private set; }

    public string Description { get; private set; } = "";

    public bool Enabled { get; private set; }

    private List<PermissionRelation> permissions = new();

    public IReadOnlyCollection<PermissionRelation> Permissions => permissions;

    private List<RolePermission> rolePermissions = new();

    public IReadOnlyCollection<RolePermission> RolePermissions => rolePermissions;

    private List<UserPermission> userPermissions = new();

    public IReadOnlyCollection<UserPermission> UserPermissions => userPermissions;

    private List<TeamPermission> teamPermissions = new();

    public IReadOnlyCollection<TeamPermission> TeamPermissions => teamPermissions;

    public Permission(int systemId, string appId, string name, string code, string url,
        string icon, PermissionTypes type, string description)
    {
        SystemId = systemId;
        AppId = appId;
        Name = name;
        Code = code;
        Url = url;
        Icon = icon;
        Type = type;
        Description = description;
    }

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
        if (permissions.Any())
        {
            throw new UserFriendlyException("current permission can`t delete,because PermissionItems not empty!");
        }
    }

    public void BindApiPermission(params Guid[] childrenId)
    {
        if (Type == PermissionTypes.Api)
        {
            throw new UserFriendlyException("the permission of api type can`t bind api permission");
        }
        foreach (var childId in childrenId)
        {
            permissions.Add(new PermissionRelation(childId));
        }
    }

    public void MoveParent(Guid parentId)
    {
        if (Type == PermissionTypes.Api)
        {
            throw new UserFriendlyException("the permission of api type can`t set parent");
        }
        ParentId = parentId;
    }

    //todo change to property field
    //eg. public DateTime HireDate { get; set => field = value.Date; }
    public void SetEnabled(bool enabled)
    {
        Enabled = enabled;
    }
}

public enum PermissionType
{
    Menu,
    Element,
    Api,
    Data
}