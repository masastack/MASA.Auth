﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class RegisterInputModelValidator : AbstractValidator<RegisterInputModel>
{
    public RegisterInputModelValidator()
    {
        When(login => login.EmailRegister, () =>
        {
            RuleFor(x => x.Email).NotEmpty().Matches(LoginOptions.EmailRegular);
            RuleFor(x => x.EmailCode).NotEmpty().Must(x => x >= 100000 && x <= 999999);
        }).Otherwise(() =>
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(LoginOptions.PhoneRegular);
            RuleFor(x => x.SmsCode).NotEmpty().Must(x => x >= 100000 && x <= 999999);
        });
        When(login => !string.IsNullOrEmpty(login.Password), () =>
        {
            RuleFor(x => x.Password).MinimumLength(6).Equal(x => x.ConfirmPassword).WithMessage("Password not equal ConfirmPassword");
        });
    }
}