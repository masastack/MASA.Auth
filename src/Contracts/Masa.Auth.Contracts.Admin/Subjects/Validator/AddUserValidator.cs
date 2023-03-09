// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddUserValidator : MasaAbstractValidator<AddUserDto>
{
    public AddUserValidator()
    {
        RuleFor(user => user.DisplayName)
           .NotEmpty().WithMessage("NickName is required")
           .Required().ChineseLetterNumber().MaximumLength(50).OverridePropertyName("NickName");
        WhenNotNullOrEmpty(user => user.Name, ruleBuilder => ruleBuilder.ChineseLetterNumber().MinimumLength(2).MaximumLength(50));
        RuleFor(user => user.PhoneNumber).Required().Phone();
        RuleFor(user => user.Email).Email();
        WhenNotNullOrEmpty(user => user.IdCard, ruleBuilder => ruleBuilder.IdCard());
        WhenNotNullOrEmpty(user => user.CompanyName, ruleBuilder => ruleBuilder.ChineseLetterNumber().MinimumLength(2).MaximumLength(50));
        WhenNotNullOrEmpty(user => user.Position, ruleBuilder => ruleBuilder.ChineseLetterNumber().MinimumLength(2).MaximumLength(16));
        RuleFor(user => user.Account).Matches("^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z0-9@.]+$")
                                     .WithMessage("Can only input chinese and letter and number and @ of {PropertyName}")
                                     .MinimumLength(8)
                                     .MaximumLength(50);
        RuleFor(user => user.Password).Required().AuthPassword();
        WhenNotNullOrEmpty(user => user.Department, ruleBuilder => ruleBuilder.ChineseLetterNumber().MinimumLength(2).MaximumLength(16));
        RuleFor(user => user.Avatar).Url().Required();
    }
}

