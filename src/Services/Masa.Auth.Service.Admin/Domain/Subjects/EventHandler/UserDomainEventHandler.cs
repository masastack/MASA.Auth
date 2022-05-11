// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class UserDomainEventHandler
{
    readonly IAutoCompleteClient _autoCompleteClient;

    public UserDomainEventHandler(IAutoCompleteClient autoCompleteClient)
    {
        _autoCompleteClient = autoCompleteClient;
    }

    [EventHandler]
    public async Task SetUserAsync(SetUserDomainEvent userEvent)
    {
        var user = userEvent.user;
        var response = await _autoCompleteClient.SetAsync<UserSelectDto, Guid>(new List<UserSelectDto>
        {
            new (user.Id, user.Name, user.Account, user.PhoneNumber, user.Email, user.Avatar)
        });
    }

    [EventHandler]
    public async Task RemoveUserAsync(RemoveUserDomainEvent userEvent)
    {
        var response = await _autoCompleteClient.DeleteAsync(userEvent.userId);
    }
}
