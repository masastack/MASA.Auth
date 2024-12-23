// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class RemoveUserDto
{
    public Guid Id { get; set; }

    public RemoveUserDto(Guid id)
    {
        Id = id;
    }
}

