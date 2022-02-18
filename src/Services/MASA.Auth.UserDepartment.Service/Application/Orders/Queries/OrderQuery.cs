using MASA.Auth.UserDepartment.Domain.Aggregate.Orders;

namespace MASA.Auth.UserDepartment.Application.Orders.Queries
{
    public record OrderQuery : DomainQuery<List<Order>>
    {
        public override List<Order> Result { get; set; } = new();
    }
}