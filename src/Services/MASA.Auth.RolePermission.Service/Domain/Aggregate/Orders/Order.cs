namespace MASA.Auth.RolePermission.Service.Domain.Aggregate.Orders
{
    public class Order : AggregateRoot<int>
    {
        public Order()
        {
            Items = new List<OrderItem>();
        }

        public DateTimeOffset CreationTime { get; set; } = DateTimeOffset.Now;

        public string OrderNumber { get; set; } = default!;

        public string Address { get; set; } = default!;

        public List<OrderItem> Items { get; set; }
    }
}