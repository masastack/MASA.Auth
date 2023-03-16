// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class LdapDetailValidator : MasaAbstractValidator<LdapDetailDto>
{
    public LdapDetailValidator()
    {
        WhenNotEmpty(c => c.ServerPort.ToString(), r => r.Port());
        RuleFor(l => l.ServerAddress).Required();
        RuleFor(l => l.RootUserDn).Required();
        RuleFor(l => l.RootUserPassword).Required();
        RuleFor(l => l.BaseDn).MinimumLength(3);
    }
}
