// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class ThirdPartyUserDomainService : DomainService
{
    public ThirdPartyUserDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task AddBeforeAsync(AddThirdPartyUserBeforeDomainEvent userEvent)
    {
        await EventBus.PublishAsync(userEvent);
    }
}