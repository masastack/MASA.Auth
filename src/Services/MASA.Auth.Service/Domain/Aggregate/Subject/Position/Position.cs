namespace MASA.Auth.Service.Domain.Aggregate
{
    public class Position : AuditAggregateRoot<Guid, Guid>
    {
        public string Name { get; set; }

        public Position()
        {
            Name = "";
        }

        public Position(string name, string code)
        {
            Name = name;
        }
    }
}
