namespace MASA.Auth.UserDepartment.Domain.Aggregate
{
    public class TeamRole : AuditAggregateRoot<Guid, Guid>
    {
        public Guid TeamId { get; set; }
        
        public Guid RoleId { get; set; }
    }
}
