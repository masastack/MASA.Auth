// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class ThirdPartyUserDomainEventHandler
{
    readonly IEventBus _eventBus;

    public ThirdPartyUserDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [EventHandler(1)]
    public async Task UpsertUser(AddThirdPartyUserBeforeDomainEvent thirdPartyUserDomainEvent)
    {
        var user = thirdPartyUserDomainEvent.User;
        var upsertUserCommand = new UpsertUserCommand(user);
        await _eventBus.PublishAsync(upsertUserCommand);
        thirdPartyUserDomainEvent.Result = upsertUserCommand.Result;
    }
}
