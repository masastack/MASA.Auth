// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister.Model;

public class RegisterValidator : AbstractValidator<RegisterModel>
{
    public RegisterValidator()
    {
        RuleFor(register => register.Account).Required().ChineseLetterNumber();

        RuleFor(register => register.Password).Required()
                                      .Matches(@"^\S*(?=\S{8,})(?=\S*\d)(?=\S*[A-Za-z])\S*$")
                                      .WithMessage("Password must contain numbers and letter, and not less than 8 digits")
                                      .MaxLength(30);

        RuleFor(register => register.ConfirmPassword)
          .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.ConfirmPassword)))
          .Must((register, value) => register.ConfirmPassword == register.Password)
          .WithMessage("The password is inconsistent with the confirm password")
          .Matches(@"^\s{0}$|^\S*(?=\S{8,})(?=\S*\d)(?=\S*[A-Za-z])\S*$")
          .WithMessage("Password must contain numbers and letter, and not less than 8 digits")
          .MaxLength(30);

        RuleFor(register => register.DisplayName)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.DisplayName)))
            .ChineseLetter().MaxLength(20);

        RuleFor(register => register.Name)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.Name)))
            .ChineseLetter().MaxLength(20);

        RuleFor(register => register.PhoneNumber)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.PhoneNumber)))
            .Phone();

        RuleFor(register => register.Email)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.Email)))
            .Email();

        RuleFor(register => register.IdCard)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.IdCard)))
            .IdCard();
    }
}

