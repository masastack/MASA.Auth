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
            RuleFor(x => x.SmsCode).NotEmpty().Must(x => x >= LoginOptions.CodeMinimum && x <= LoginOptions.CodeMaximum);
        }).Otherwise(() =>
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(4);
        });
        RuleFor(x => x.Environment).NotEmpty();
    }
}