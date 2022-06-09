// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations;

public class RemovePositionDto
{
    public Guid Id { get; set; }

    public RemovePositionDto(Guid id)
    {
        Id = id;
    }
}
