// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Events;

public record AddStaffBeforeDomainEvent(AddUserDto User, string? Position) : Event
{
    public Guid UserId { get; set; }

    public Guid PositionId { get; set; }
}

