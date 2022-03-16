namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

public abstract class Secret : Entity<int>
{
    public string Description { get; } = "";

    public string Value { get; } = "";

    public DateTime? Expiration { get; }

    public string Type { get; } = "SharedSecret";

    public DateTime Created { get; } = DateTime.UtcNow;
}
