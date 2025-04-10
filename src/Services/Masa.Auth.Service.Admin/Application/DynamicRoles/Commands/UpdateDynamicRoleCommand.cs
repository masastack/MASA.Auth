// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.DynamicRoles.Commands;

public record UpdateDynamicRoleCommand(Guid Id, DynamicRoleUpsertDto UpsertDto) : Command
{
}
