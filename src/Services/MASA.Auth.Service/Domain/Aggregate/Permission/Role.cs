namespace MASA.Auth.Service.Domain.Aggregate;

public class Role : AuditAggregateRoot<int, int>
{
    public string Name { get; set; } = "";

    public string Code { get; set; } = "";

    public string Description { get; set; } = "";

    public State State { get; set; }

    /// <summary>
    /// user role limit count
    /// </summary>
    public int Limit { get; set; }

    private List<RolePermission> rolePermissions = new();

    public IReadOnlyCollection<RolePermission> RolePermissions => rolePermissions;

    private List<RoleItem> roleItems = new();

    public IReadOnlyCollection<RoleItem> RoleItems => roleItems;
}
