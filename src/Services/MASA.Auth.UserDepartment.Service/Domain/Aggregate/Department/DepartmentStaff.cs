namespace MASA.Auth.UserDepartment.Domain.Aggregate
{
    public class DepartmentStaff : AuditAggregateRoot<Guid, Guid>
    {
        public Guid StaffId { get; set; }

        public Guid DepartmentId { get; set; }
    }
}