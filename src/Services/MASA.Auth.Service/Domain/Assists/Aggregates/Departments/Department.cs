namespace MASA.Auth.Service.Domain.Assists.Aggregates
{
    public class Department : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; private set; }

        public int Sort { get; private set; }

        public Guid? ParentId { get; private set; }

        public string? Describe { get; private set; }

        private Department()
        {
            Name = "";
        }

        public Department(string name, int sort) => (Name, Sort) = (name, sort);
    }
}
