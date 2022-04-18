namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class Role : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    private List<RolePermission> _permissions = new();
    private List<RoleRelation> _childrenRoles = new();
    private List<RoleRelation> _parentRoles = new();
    private List<UserRole> _users = new();
    private List<TeamRole> _teams = new();
    private User? _creatorUser;
    private User? _modifierUser;
    private int _limit;

    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool IsDeleted { get; private set; }

    public bool Enabled { get; private set; }

    public int Limit
    {
        get => _limit;
        set
        {
            if (value < 0)
                throw new UserFriendlyException("This operation cannot be completed due to role Limit restrictions");

            _limit = value;
        }
    }

    public int AvailableQuantity { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions;

    public IReadOnlyCollection<RoleRelation> ChildrenRoles => _childrenRoles;

    public IReadOnlyCollection<RoleRelation> ParentRoles => _parentRoles;

    public IReadOnlyCollection<UserRole> Users => _users;

    public IReadOnlyCollection<TeamRole> Teams => _teams;

    public User? CreatorUser => _creatorUser ?? LazyLoader?.Load(this, ref _creatorUser);

    public User? ModifierUser => _modifierUser ?? LazyLoader?.Load(this, ref _modifierUser);

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
        AvailableQuantity = Limit;
    }

    public void BindChildrenRoles(List<Guid> childrenRoles)
    {
        _childrenRoles.Clear();
        _childrenRoles.AddRange(childrenRoles.Select(roleId => new RoleRelation(roleId, default)));
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

    public void UpdateAvailableQuantity(int availableQuantity)
    {
        AvailableQuantity = availableQuantity;
    }
}
