namespace MASA.Auth.RolePermission.Service.Domain.Aggregate.Orders
{
    public class OrderItem : Entity<int>
    {
        public int ProductId { get; set; }

        public float Price { get; set; }
    }
}