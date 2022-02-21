namespace MASA.Auth.Service.Domain.Aggregate
{
    public class TeamRole : AuditAggregateRoot<Guid, Guid>
    {
        public Guid TeamId { get; private set;}

        public Guid RoleId { get; private set;}

        public TeamRole(Guid teamId, Guid roleId)
        {
            TeamId = teamId;
            RoleId = roleId;
        }
    }
}
