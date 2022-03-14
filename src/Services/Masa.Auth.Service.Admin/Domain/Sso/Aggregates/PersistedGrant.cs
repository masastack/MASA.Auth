namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class PersistedGrant : AggregateRoot<int>
{
    public string Key { get; set; } = null!;

    public string Type { get; set; } = String.Empty;

    public string SubjectId { get; set; } = String.Empty;

    public string SessionId { get; set; } = String.Empty;

    public string ClientId { get; set; } = String.Empty;

    public string Description { get; set; } = String.Empty;

    public DateTime CreationTime { get; set; }

    public DateTime? Expiration { get; set; }

    public DateTime? ConsumedTime { get; set; }

    public string Data { get; set; } = String.Empty;
}
