// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class LoginInputModelValidator : AbstractValidator<LoginInputModel>
{
    public LoginInputModelValidator(I18n i18N)
    {
        When(login => login.PhoneLogin, () =>
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(i18N.T("PhoneRequired"))
            .Matches(LoginOptions.PhoneRegular);
            RuleFor(x => x.SmsCode).NotEmpty().WithMessage(i18N.T("SmsRequired"))
            .Must(x => x >= LoginOptions.CodeMinimum && x <= LoginOptions.CodeMaximum);
        }).Otherwise(() =>
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage(i18N.T("UserNameRequired"));
            RuleFor(x => x.Password).NotEmpty().WithMessage(i18N.T("PwdRequired"));
        });
        RuleFor(x => x.Environment).NotEmpty();
    }
}