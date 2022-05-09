// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister.Model;

public class RegisterValidator : AbstractValidator<RegisterModel>
{
    public RegisterValidator()
    {
        RuleFor(register => register.Account).Required().ChineseLetterNumber();
        RuleFor(register => register.Password).Required().LetterNumber();
        RuleFor(register => register.DisplayName).Must((register, displayName) => 
        {
            if(register.CheckRequired(nameof(RegisterModel.DisplayName)))
            {
                return string.IsNullOrEmpty(displayName) is false;
            }
            return true;
        }).ChineseLetter().MinLength(1).MaxLength(20);
        RuleFor(register => register.Name).Must((register, name) =>
        {
            if (register.CheckRequired(nameof(RegisterModel.Name)))
            {
                return string.IsNullOrEmpty(name) is false;
            }
            return true;
        }).ChineseLetter().MaxLength(20);
        RuleFor(register => register.PhoneNumber).Must((register, phoneNumber) =>
        {
            if (register.CheckRequired(nameof(RegisterModel.PhoneNumber)))
            {
                return string.IsNullOrEmpty(phoneNumber) is false;
            }
            return true;
        }).Phone();
        RuleFor(register => register.Email).Must((register, email) =>
        {
            if (register.CheckRequired(nameof(RegisterModel.Email)))
            {
                return string.IsNullOrEmpty(email) is false;
            }
            return true;
        }).Email();
        RuleFor(register => register.IdCard).Must((register, idCard) =>
        {
            if (register.CheckRequired(nameof(RegisterModel.IdCard)))
            {
                return string.IsNullOrEmpty(idCard) is false;
            }
            return true;
        }).IdCard();
    }
}

