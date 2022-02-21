namespace MASA.Auth.Service.Domain.Aggregate
{
    public class TeamStaff : AuditAggregateRoot<Guid, Guid>
    {
        public Guid TeamId { get; private set; }

        public Guid StaffId { get; private set; }

        public TeamStaff(Guid teamId, Guid staffId)
        {
            TeamId = teamId;
            StaffId = staffId;
        }
    }

    public enum TeamStaffType
    {
        Member,
        Admin,
    }
}
