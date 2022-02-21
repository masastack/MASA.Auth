namespace MASA.Auth.Service.Domain.Aggregate
{
    public class Team : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; private set; }

        public string? Avatar { get; private set; }

        public string? Describe { get; private set; }

        public Team()
        {
            Name = "";
        }

        public Team(string name, string? avatar, string? describe)
        {
            Name = name;
            Avatar = avatar;
            Describe = describe;
        }
    }
}
