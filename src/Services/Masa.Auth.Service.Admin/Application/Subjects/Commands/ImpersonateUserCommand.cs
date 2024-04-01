// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public record ImpersonateUserCommand(Guid UserId, bool IsBackToImpersonator) : Command
{
    public ImpersonateOutput Result { get; set; } = new();
}