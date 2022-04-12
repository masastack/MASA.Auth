namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class Role : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    private List<RolePermission> _permissions = new();
    private List<RoleRelation> _childrenRoles = new();
    private List<UserRole> _users = new();
    private User? _creator;
    private User? _modifier;

    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool IsDeleted { get; private set; }

    public bool Enabled { get; private set; }

    public int Limit { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions;

    public IReadOnlyCollection<RoleRelation> ChildrenRoles => _childrenRoles;

    public IReadOnlyCollection<UserRole> Users => _users;

    public User? CreatorUser => _creator ?? LazyLoader?.Load(this, ref _creator);    

    public User? ModifierUser => _modifier ?? LazyLoader?.Load(this, ref _modifier);

    private ILazyLoader? LazyLoader { get; set; }

    public Role(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
        Name = "";
        Description = "";
    }

    public Role(string name, string description) : this(name, description, true, 1)
    {
    }

    public Role(string name, string description, bool enabled, int limit)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
        Limit = limit;
    }

    public void BindChildrenRoles(List<Guid> childrenRoles)
    {
        _childrenRoles.Clear();
        _childrenRoles.AddRange(childrenRoles.Select(roleId => new RoleRelation(roleId)));
    }

    public void BindPermissions(List<Guid> permissions)
    {
        _permissions.AddRange(permissions.Select(roleId => new RolePermission(roleId)));
    }

    public void Update(string name, string description, bool enabled, int limit)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
        Limit = limit;
    }
}
