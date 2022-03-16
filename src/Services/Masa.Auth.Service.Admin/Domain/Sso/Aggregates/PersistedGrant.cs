namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class PersistedGrant : AggregateRoot<int>
{
    public string Key { get; } = null!;

    public string Type { get; } = string.Empty;

    public string SubjectId { get; } = string.Empty;

    public string SessionId { get; } = string.Empty;

    public string ClientId { get; } = string.Empty;

    public string Description { get; } = string.Empty;

    public DateTime CreationTime { get; }

    public DateTime? Expiration { get; }

    public DateTime? ConsumedTime { get; }

    public string Data { get; } = string.Empty;
}
