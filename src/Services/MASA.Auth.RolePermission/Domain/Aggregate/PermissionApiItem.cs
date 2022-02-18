namespace MASA.Auth.RolePermission.Domain.Aggregate
{
    public class PermissionApiItem : Entity<int>
    {
        public int ChildApiId { get; set; }

        public Permission Permission { get; set; } = null!;
    }
}
