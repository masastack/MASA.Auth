namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

public abstract class Secret : Entity<int>
{
    public string Description { get; private set; } = "";

    public string Value { get; private set; } = "";

    public DateTime? Expiration { get; private set; }

    public string Type { get; private set; } = "SharedSecret";

    public DateTime Created { get; private set; } = DateTime.UtcNow;
}
