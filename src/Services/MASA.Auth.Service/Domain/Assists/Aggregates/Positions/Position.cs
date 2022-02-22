namespace MASA.Auth.Service.Domain.Assists.Aggregates
{
    public class Position : AggregateRoot<Guid>
    {
        public string Name { get; private set; } = "";

        private Position()
        {

        }

        public Position(string name) => Name = name;
    }
}
