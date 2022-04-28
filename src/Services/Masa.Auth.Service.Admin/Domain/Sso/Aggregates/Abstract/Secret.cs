namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

public abstract class Secret : AuditEntity<int, Guid>, ISoftDelete
{
    public bool IsDeleted { get; protected set; }

    public string Description { get; protected set; } = string.Empty;

    public string Value { get; protected set; } = string.Empty;

    public DateTime? Expiration { get; protected set; }

    public string Type { get; protected set; } = "SharedSecret";
}
