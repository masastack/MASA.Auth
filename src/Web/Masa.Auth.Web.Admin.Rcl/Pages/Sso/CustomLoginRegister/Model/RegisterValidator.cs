// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using FluentValidation;
using FluentValidation.Validators;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister.Model;

public class RegisterValidator : MasaAbstractValidator<RegisterModel>
{
    public RegisterValidator()
    {
        RuleFor(register => register.PhoneNumber)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.PhoneNumber))).Phone();

        _ = WhenNotEmpty(r => r.Account, rule => rule.ChineseLetterNumber());
        RuleFor(register => register.Account).RequiredIf(register => register.CheckRequired(nameof(RegisterModel.Account)));

        _ = WhenNotEmpty(r => r.Password, rule => rule.AuthPassword());
        RuleFor(register => register.Password).RequiredIf(register => register.CheckRequired(nameof(RegisterModel.Password)));

        _ = WhenNotEmpty(r => r.ConfirmPassword, rule => rule.Must((register, value) => 
            register.ConfirmPassword == register.Password).WithMessage("The password is inconsistent with the confirm password")
            .Matches(BusinessConsts.PASSWORD_REGULAR).WithMessage("Password must contain numbers and letter, and not less than 6 digits")
            .MaximumLength(30));
        RuleFor(register => register.ConfirmPassword)
          .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.Password)));

        _ = WhenNotEmpty(r => r.DisplayName, rule => rule.ChineseLetter().MaximumLength(20));
        RuleFor(register => register.DisplayName)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.DisplayName)));

        _ = WhenNotEmpty(r => r.Name, rule => rule.ChineseLetter().MaximumLength(20));
        RuleFor(register => register.Name)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.Name)));

        _ = WhenNotEmpty(r => r.Email, rule => rule.Email());
        RuleFor(register => register.Email)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.Email)));

        _ = WhenNotEmpty(r => r.IdCard, rule => rule.IdCard());
        RuleFor(register => register.IdCard)
            .RequiredIf(register => register.CheckRequired(nameof(RegisterModel.IdCard)));
    }
}

