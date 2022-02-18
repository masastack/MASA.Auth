using MASA.Auth.UserDepartment.Domain.Aggregate.Orders;

namespace MASA.Auth.UserDepartment.Domain.Events
{
    public record OrderQueryDomainEvent : DomainEvent
    {
        public List<Order> Orders { get; set; } = new();
    }
}