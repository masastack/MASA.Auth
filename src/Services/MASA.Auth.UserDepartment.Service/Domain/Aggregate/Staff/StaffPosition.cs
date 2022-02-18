namespace MASA.Auth.UserDepartment.Domain.Aggregate.Staff
{
    public class StaffPosition : AuditAggregateRoot<Guid, Guid>
    {
        public Guid StaffId { get; set; }

        public Guid? PositionId { get; set; }
    }
}
