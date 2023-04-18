// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class PasswordValidator : MasaAbstractValidator<string?>
{
    public PasswordValidator(PasswordHelper passwordHelper)
    {
        RuleFor(password => password).Required().MaximumLength(50).Custom(passwordHelper.ValidatePassword);
    }
}
