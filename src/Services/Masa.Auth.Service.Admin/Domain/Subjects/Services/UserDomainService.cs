public class UserDomainService : DomainService
{
    public UserDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task SetAsync(User user)
    {
        await EventBus.PublishAsync(new SetUserDomainEvent(user));
    }

    public async Task RemoveAsync(Guid userId)
    {
        await EventBus.PublishAsync(new RemoveUserDomainEvent(userId));
    }
}

