namespace MASA.Auth.Service.Domain.Aggregate
{
    public class StaffRole : AuditAggregateRoot<Guid, Guid>
    {
        public Guid StaffId { get; set; }

        public Guid? RoleId { get; set; }
    }
}