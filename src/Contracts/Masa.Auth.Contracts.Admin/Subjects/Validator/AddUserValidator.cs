// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddUserValidator : AbstractValidator<AddUserDto>
{
    public AddUserValidator()
    {
        RuleFor(user => user.DisplayName)
           .NotEmpty().WithMessage("NickName is required")
           .Required().ChineseLetterNumber().MaximumLength(50).OverridePropertyName("NickName");
        RuleFor(user => user.Name).ChineseLetterNumber().MaximumLength(50);
        RuleFor(user => user.PhoneNumber).Required().Phone();
        RuleFor(user => user.Email).Email();
        RuleFor(user => user.IdCard).IdCard();
        RuleFor(user => user.CompanyName).ChineseLetterNumber().MinimumLength(2).MaximumLength(50);
        RuleFor(user => user.Position).ChineseLetterNumber().MinimumLength(2).MaximumLength(16);
        RuleFor(user => user.Account).ChineseLetterNumber().MinimumLength(8).MaximumLength(50);
        RuleFor(user => user.Password).Required().AuthPassword();
        RuleFor(user => user.Department).ChineseLetterNumber().MinimumLength(2).MaximumLength(16);
        RuleFor(user => user.Avatar).Url().Required();
    }
}

