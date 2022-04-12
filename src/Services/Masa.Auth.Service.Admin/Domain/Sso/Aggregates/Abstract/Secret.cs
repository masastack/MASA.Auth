namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

public abstract class Secret : AuditEntity<int, Guid>, ISoftDelete
{
    public bool IsDeleted { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public string Value { get; private set; } = string.Empty;

    public DateTime? Expiration { get; private set; }

    public string Type { get; private set; } = "SharedSecret";
}
