namespace MASA.Auth.UserDepartment.Domain.Aggregate
{
    public class StaffRole : AuditAggregateRoot<Guid, Guid>
    {
        public Guid StaffId { get; set; }

        public Guid? RoleId { get; set; }     
    }
}