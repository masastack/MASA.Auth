namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class IdentityProvider : AuditAggregateRoot<Guid, Guid>, ISoftDelete
{
    public bool IsDeleted { get; protected set; }

    public string Name { get; protected set; } = null!;

    public string DisplayName { get; protected set; } = null!;

    public string Icon { get; protected set; } = null!;

    public IdentificationTypes IdentificationType { get; protected set; }
}
