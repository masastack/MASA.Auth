namespace MASA.Auth.UserDepartment.Domain.Aggregate.Staff
{
    public class StaffDepartment : AuditAggregateRoot<Guid, Guid>
    {
        public Guid UserId { get; set; }

        public Guid DepartmentId { get; set; }
    }
}