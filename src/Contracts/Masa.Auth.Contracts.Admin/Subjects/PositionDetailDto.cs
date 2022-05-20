// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class PositionDetailDto : PositionDto
{
    public PositionDetailDto()
    {

    }

    public PositionDetailDto(Guid id, string name) : base(id, name)
    {
    }

    public static implicit operator UpdatePositionDto(PositionDetailDto position)
    {
        return new UpdatePositionDto(position.Id, position.Name);
    }
}


