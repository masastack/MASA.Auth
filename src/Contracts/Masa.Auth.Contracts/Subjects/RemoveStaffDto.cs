// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class RemoveStaffDto
{
    public Guid Id { get; set; }

    public RemoveStaffDto(Guid id)
    {
        Id = id;
    }
}

