// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateUserAuthorizationDto
{
    public Guid Id { get; set; }

    public List<Guid> Roles { get; set; }

    public List<SubjectPermissionRelationDto> Permissions { get; set; }

    public UpdateUserAuthorizationDto()
    {
        Roles = new();
        Permissions = new();
    }

    public UpdateUserAuthorizationDto(Guid id, IEnumerable<Guid> roles, IEnumerable<SubjectPermissionRelationDto> permissions)
    {
        Id = id;
        Roles = roles.ToList();
        Permissions = permissions.ToList();
    }
}

