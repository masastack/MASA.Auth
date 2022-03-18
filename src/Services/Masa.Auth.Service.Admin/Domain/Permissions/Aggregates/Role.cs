namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class Role : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; } = "";

    public string Code { get; private set; } = "";

    /// <summary>
    /// user role limit count
    /// </summary>
    public int Limit { get; private set; }

    public string Description { get; private set; } = "";

    public bool Enabled { get; private set; }

    private List<RolePermission> rolePermissions = new();

    public IReadOnlyCollection<RolePermission> RolePermissions => rolePermissions;

    private List<RoleRelation> roleItems = new();

    public IReadOnlyCollection<RoleRelation> RoleItems => roleItems;
}
