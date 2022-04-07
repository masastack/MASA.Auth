public class StaffDomainService : DomainService
{
    public StaffDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task AddStaffAsync(AddStaffDto staff)
    {
        await EventBus.PublishAsync(new AddStaffDomainEvent(staff));
    }

    public async Task UpdateStaffAsync(UpdateStaffDto staff)
    {
        await EventBus.PublishAsync(new UpdateStaffDomainEvent(staff));
    }
}
