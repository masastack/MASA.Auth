// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class RemoveUserClaimDto
{
    public int Id { get; set; }

    public RemoveUserClaimDto(int id)
    {
        Id = id;
    }
}

