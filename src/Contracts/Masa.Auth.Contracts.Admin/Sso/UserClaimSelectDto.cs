// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class UserClaimSelectDto : UserClaimDto
{
    public UserClaimSelectDto(int id, string name, string description) : base(id, name, description, default)
    {
    }
}

