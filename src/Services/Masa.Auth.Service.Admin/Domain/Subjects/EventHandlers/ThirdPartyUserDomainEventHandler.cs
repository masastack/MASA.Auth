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
    public async Task AddUser(AddThirdPartyUserBeforeDomainEvent thirdPartyUserDomainEvent)
    {
        var user = thirdPartyUserDomainEvent.User;
        var addUserCommand = new AddUserCommand(user, true);
        await _eventBus.PublishAsync(addUserCommand);
        thirdPartyUserDomainEvent.UserId = addUserCommand.NewUser.Id;
    }
}
