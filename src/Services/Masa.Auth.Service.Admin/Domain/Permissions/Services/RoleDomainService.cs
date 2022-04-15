using Masa.Auth.Service.Admin.Domain.Permissions.Events;

public class RoleDomainService : DomainService
{
    public RoleDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task UpdateRoleLimitAsync(IEnumerable<Guid> roles)
    {
        roles = roles.Distinct();
        await EventBus.PublishAsync(new UpdateRoleLimitDomainEvent(roles));
    }
}
