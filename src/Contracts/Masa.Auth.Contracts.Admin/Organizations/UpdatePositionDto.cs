// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations;

public class UpdatePositionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public UpdatePositionDto()
    {
    }

    public UpdatePositionDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
