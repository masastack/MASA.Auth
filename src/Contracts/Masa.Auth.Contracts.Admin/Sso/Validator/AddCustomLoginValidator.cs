// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddCustomLoginValidator : AbstractValidator<AddCustomLoginDto>
{
    public AddCustomLoginValidator()
    {
        RuleFor(CustomLogin => CustomLogin.Name).Required().ChineseLetterNumber().MinLength(2).MaxLength(50);
        RuleFor(CustomLogin => CustomLogin.Title).Required().ChineseLetterNumber().MinLength(2).MaxLength(50);
    }
}

