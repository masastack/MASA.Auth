// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class StaffDomainService : DomainService
{
    public StaffDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task AddStaffBeforeAsync(AddStaffBeforeDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }

    public async Task AddStaffAfterAsync(AddStaffAfterDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }

    public async Task UpdateStaffBeforeAsync(UpdateStaffBeforeDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }

    public async Task UpdateStaffAfterAsync(UpdateStaffAfterDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }

    public async Task RemoveStaffAsync(RemoveStaffDomainEvent staffEvent)
    {
        await EventBus.PublishAsync(staffEvent);
    }
}
