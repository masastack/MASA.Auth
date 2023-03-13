// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class LoginInputModelValidator : AbstractValidator<LoginInputModel>
{
    public LoginInputModelValidator(I18n i18n)
    {
        When(login => login.PhoneLogin, () =>
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(i18n.T("PhoneRequired"))
            .Matches(LoginOptions.PhoneRegular).WithMessage(i18n.T("PhoneSpecInvalid"));
            RuleFor(x => x.SmsCode).NotEmpty().WithMessage(i18n.T("SmsRequired"));
        }).Otherwise(() =>
        {
            RuleFor(x => x.Account).NotEmpty().WithMessage(i18n.T("AccountRequired"));
            RuleFor(x => x.Password).NotEmpty().WithMessage(i18n.T("PwdRequired"));
        });
        RuleFor(x => x.Environment).NotEmpty();
    }
}
