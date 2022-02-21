namespace MASA.Auth.Service.Domain.Subjects.Aggregates
{
    public class StaffPosition : AuditAggregateRoot<Guid, Guid>
    {
        public Guid StaffId { get; set; }

        public Guid? PositionId { get; set; }
    }
}
