namespace MASA.Auth.UserDepartment.Domain.Aggregate.Teams
{
    public class Team : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; set; }
        
        public string? Avatar { get; set; }

        public string? Describe { get; set; }

        public Team(string name)
        {
            Name = name;
        }
    }
}
