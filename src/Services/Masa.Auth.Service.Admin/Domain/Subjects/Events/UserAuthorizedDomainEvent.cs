// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Events;

public record UserAuthorizedDomainEvent(string AppId, string Code, Guid UserId) : Event
{
    public bool Authorized { get; set; }

    public Guid PermissionId { get; set; }

    public List<Guid> Roles { get; set; } = new();
}
