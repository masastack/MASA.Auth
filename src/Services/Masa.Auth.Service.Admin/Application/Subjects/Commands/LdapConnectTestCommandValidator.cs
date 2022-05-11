// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class LdapConnectTestCommandValidator : AbstractValidator<LdapUpsertCommand>
{
    public LdapConnectTestCommandValidator()
    {
        RuleFor(command => command.LdapDetailDto).SetValidator(new LdapDetailValidator());
    }
}
