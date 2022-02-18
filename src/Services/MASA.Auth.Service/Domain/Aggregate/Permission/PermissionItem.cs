namespace MASA.Auth.Service.Domain.Aggregate
{
    public class PermissionItem : Entity<int>
    {
        public int ChildPermissionId { get; set; }

        public Permission Permission { get; set; } = null!;
    }
}
