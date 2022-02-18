namespace MASA.Auth.Service.Domain.Aggregate
{
    public class Team : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; set; }

        public string? Avatar { get; set; }

        public string? Describe { get; set; }

        public Team()
        {
            Name = "";
        }

        public Team(string name)
        {
            Name = name;
        }
    }
}
