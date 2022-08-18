// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class StaffDomainService : DomainService
{
    public StaffDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task AddBeforeAsync(AddStaffBeforeDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }

    public async Task AddAfterAsync(AddStaffAfterDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }

    public async Task UpdateBeforeAsync(UpdateStaffBeforeDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }

    public async Task UpdateAfterAsync(UpdateStaffAfterDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }

    public async Task RemoveAsync(RemoveStaffDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }
}
