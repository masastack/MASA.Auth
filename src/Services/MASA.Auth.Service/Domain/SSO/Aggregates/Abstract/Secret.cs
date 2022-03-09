namespace Masa.Auth.Service.Domain.Sso.Aggregates.Abstract;

public abstract class Secret : Entity<int>
{
    public string Description { get; set; } = "";

    public string Value { get; set; } = "";

    public DateTime? Expiration { get; set; }

    public string Type { get; set; } = "SharedSecret";

    public DateTime Created { get; set; } = DateTime.UtcNow;
}
