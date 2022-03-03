namespace MASA.Auth.Service.Domain.Organization.Aggregates;

public class Position : AggregateRoot<Guid>
{
    public string Name { get; private set; } = "";

    public Position(string name) => Name = name;
}

