namespace MASA.Auth.Service.Domain.Subjects.Aggregates
{
    public class Position : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; private set; } = "";

        private Position()
        {

        }

        public Position(string name) => Name = name;
    }
}
