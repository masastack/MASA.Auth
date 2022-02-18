using MASA.Auth.UserDepartment.Domain.Aggregate.Orders;

namespace MASA.Auth.UserDepartment.Application.Orders.Commands
{
    public record OrderCreateCommand : DomainCommand
    {
        public List<OrderItem> Items { get; set; } = new();
    }
}