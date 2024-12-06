// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
