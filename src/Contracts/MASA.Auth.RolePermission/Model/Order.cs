namespace MASA.Auth.RolePermission.Contracts.Order.Model
{
    public class Order
    {
        public DateTime CreationTime { get; set; }

        public int Id { get; set; }

        public string OrderNumber { get; set; } = "";

        public string Address { get; set; } = default!;
    }
}