// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(user => user.DisplayName)
           .NotEmpty().WithMessage("NickName is required")
           .Required().ChineseLetterNumber().MaximumLength(50).OverridePropertyName("NickName");
        When(staff => !string.IsNullOrEmpty(staff.Name), () => RuleFor(staff => staff.Name).ChineseLetter().MinimumLength(2).MaximumLength(20));
        RuleFor(user => user.PhoneNumber).Required().Phone();
        RuleFor(user => user.Email).Email();
        When(staff => !string.IsNullOrEmpty(staff.IdCard), () => RuleFor(staff => staff.IdCard).IdCard());
        When(staff => !string.IsNullOrEmpty(staff.CompanyName), () => RuleFor(staff => staff.CompanyName).ChineseLetterNumber().MaximumLength(50));
        When(staff => !string.IsNullOrEmpty(staff.Position), () => RuleFor(staff => staff.Position).ChineseLetterNumber().MaximumLength(20));
        RuleFor(user => user.Account).Required()
                                     .Matches("^\\s{0}$|^[\u4e00-\u9fa5_a-zA-Z0-9@.]+$")
                                     .WithMessage("Can only input chinese and letter and number and @ of {PropertyName}")
                                     .MinimumLength(8)
                                     .MaximumLength(50);
        When(u => !string.IsNullOrEmpty(u.Department), () => RuleFor(user => user.Department).ChineseLetterNumber().MinimumLength(2).MaximumLength(16));
        RuleFor(user => user.Avatar).Url().Required();
    }
}

