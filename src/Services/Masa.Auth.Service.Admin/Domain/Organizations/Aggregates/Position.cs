namespace Masa.Auth.Service.Domain.Organizations.Aggregates;

public class Position : AggregateRoot<Guid>
{
    public string Name { get; private set; }

    public Position(string name) => Name = name;
}

