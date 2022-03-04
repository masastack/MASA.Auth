namespace MASA.Auth.Service.Domain.Organizations.Aggregates;

public class Position : AggregateRoot<Guid>
{
    public string Name { get; private set; }

    private Position()
    {
        Name = "";
    }

    public Position(string name) => Name = name;
}

