// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class RemoveApiResourceDto
{
    public int Id { get; set; }

    public RemoveApiResourceDto(int id)
    {
        Id = id;
    }
}

