// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class ResetUserPasswordValidator : MasaAbstractValidator<ResetUserPasswordDto>
{
    public ResetUserPasswordValidator(PasswordValidator passwordValidator)
    {
        RuleFor(staff => staff.Password).SetValidator(passwordValidator);
    }
}
