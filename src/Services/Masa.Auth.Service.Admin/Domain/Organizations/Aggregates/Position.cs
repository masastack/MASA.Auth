namespace Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;

public class Position : AggregateRoot<Guid>
{
    public string Name { get; private set; }

    public Position(string name) => Name = name;
}

