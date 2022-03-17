namespace Masa.Auth.Service.Domain.Permissions.Aggregates;

public class Role : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool Enabled { get; private set; }

    private List<RolePermission> rolePermissions = new();

    public IReadOnlyCollection<RolePermission> RolePermissions => rolePermissions;

    private List<RoleRelation> roleItems = new();

    public IReadOnlyCollection<RoleRelation> RoleItems => roleItems;

    public Role(string name, string description, bool enabled)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
    }

    public void BindChildrenRoles(List<Guid> childrenRoles)
    {
        roleItems.AddRange(childrenRoles.Select(roleId=> new RoleRelation(roleId)));
    }

    public void BindPermissions(List<Guid> permissions)
    {
        rolePermissions.AddRange(permissions.Select(roleId => new RolePermission(roleId)));
    }

    public void Update()
    {

    }
}
