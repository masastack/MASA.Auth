// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class LdapDetailValidator : AbstractValidator<LdapDetailDto>
{
    public LdapDetailValidator()
    {
        RuleFor(l => l.ServerPort.ToString()).Port();
        RuleFor(l => l.ServerAddress).Required();
        RuleFor(l => l.RootUserDn).Required();
        RuleFor(l => l.RootUserPassword).Required();
        RuleFor(l => l.BaseDn).MinLength(3);
    }
}
