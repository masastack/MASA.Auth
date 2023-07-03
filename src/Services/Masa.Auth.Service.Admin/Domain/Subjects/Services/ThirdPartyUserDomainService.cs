// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Services;

public class ThirdPartyUserDomainService : DomainService
{

    readonly UserDomainService _userDomainService;

    public ThirdPartyUserDomainService(UserDomainService userDomainService)
    {
        _userDomainService = userDomainService;
    }

    public async Task<UserModel> AddThirdPartyUserAsync(AddThirdPartyUserDto dto)
    {
        var addUserCommand = new AddUserCommand(dto.User, true);
        await EventBus.PublishAsync(addUserCommand);
        return addUserCommand.Result.Adapt<UserModel>();
    }
}
