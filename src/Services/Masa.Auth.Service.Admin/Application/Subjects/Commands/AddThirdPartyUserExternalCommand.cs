// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record AddThirdPartyUserExternalCommand(AddThirdPartyUserModel ThirdPartyUser, bool WhenExisReturn = false) : Command
{
    private UserModel? _user;

    public UserModel Result
    {
        get => _user ?? throw new UserFriendlyException("Failed to add third-party user");
        set => _user = value;
    }
}
