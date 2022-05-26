// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class UserDomainService : DomainService
{
    public UserDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task SetAsync(params User[] users)
    {
        await EventBus.PublishAsync(new SetUserDomainEvent(users.ToList()));
    }

    public async Task RemoveAsync(params Guid[] userIds)
    {
        await EventBus.PublishAsync(new RemoveUserDomainEvent(userIds.ToList()));
    }
}

