// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class LoginInputModelValidator : AbstractValidator<LoginInputModel>
{
    public LoginInputModelValidator()
    {
        When(login => login.PhoneLogin, () =>
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(LoginOptions.PhoneRegular);
            RuleFor(x => x.SmsCode).NotEmpty().Must(x => x >= 100000 && x <= 999999);
        }).Otherwise(() =>
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(4);
        });
        RuleFor(x => x.Environment).NotEmpty();
    }
}