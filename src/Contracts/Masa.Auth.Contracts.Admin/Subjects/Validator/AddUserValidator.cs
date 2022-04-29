// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddUserValidator : AbstractValidator<AddUserDto>
{
    public AddUserValidator()
    {
        RuleFor(user => user.DisplayName).ChineseLetter().MinLength(1).MaxLength(20);
        RuleFor(user => user.Name).ChineseLetter().MaxLength(20);
        RuleFor(user => user.PhoneNumber).Phone();
        RuleFor(user => user.Email).Email();
        RuleFor(user => user.IdCard).IdCard();
        //RuleFor(user => user.CompanyName).ChineseLetter().MinLength(1).MaxLength(50);
        //RuleFor(user => user.Position).ChineseLetterNumber().MaxLength(20);
        RuleFor(user => user.Account).Required().ChineseLetterNumber();
        RuleFor(user => user.Password).Required().LetterNumber();
    }
}

