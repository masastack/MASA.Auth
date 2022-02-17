namespace MASA.Auth.RolePermission.Service.Application.Orders.Queries
{
    public record OrderQuery : DomainQuery<List<Order>>
    {
        public override List<Order> Result { get; set; } = new();
    }
}