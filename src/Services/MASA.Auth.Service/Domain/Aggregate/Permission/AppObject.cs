namespace MASA.Auth.Service.Domain.Aggregate;

public class AppObject : AuditAggregateRoot<int, int>
{
    public string AppId { get; set; }

    public string Code { get; private set; }

    public string Name { get; private set; }

    public State State { get; private set; }
}

