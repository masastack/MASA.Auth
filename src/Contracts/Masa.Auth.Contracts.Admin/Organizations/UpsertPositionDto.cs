// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations;

public class UpsertPositionDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public UpsertPositionDto()
    {
    }

    public UpsertPositionDto(Guid id, string name)
    {
        Name = name;
    }
}
