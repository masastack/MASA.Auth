namespace MASA.Auth.UserDepartment.Domain.Aggregate.Department
{
    public class DepartmentPosition : AuditAggregateRoot<Guid, Guid>
    {
        public Guid Department { get; set; }

        public Guid PositionId { get; set; }
    }
}
