namespace MASA.Auth.Service.Domain.Aggregate
{
    public class Position : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; private set; }

        public Position() => Name = "";

        public Position(string name) => Name = name;
    }
}
