namespace MASA.Auth.Service.Domain.Subjects.Aggregates;

    public class StaffPosition : Entity<Guid>
    {
        public Guid StaffId { get; private set; }

        public Guid PositionId { get; private set; }

    public StaffPosition(Guid staffId, Guid positionId)
    {
        StaffId = staffId;
        PositionId = positionId;
    }
}

