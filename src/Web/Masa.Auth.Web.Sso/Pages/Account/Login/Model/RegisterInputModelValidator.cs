// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class RegisterInputModelValidator : AbstractValidator<RegisterInputModel>
{
    public RegisterInputModelValidator(I18n i18n)
    {
        When(login => login.EmailRegister, () =>
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(i18n.T("EmailRequired"))
		.Matches(LoginOptions.EmailRegular).WithMessage(i18n.T("EmailSpecInvalid"));
            RuleFor(x => x.EmailCode).NotEmpty().WithMessage(i18n.T("EmailCodeRequired"));
        }).Otherwise(() =>
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage(i18n.T("PhoneRequired"))
		.Matches(LoginOptions.PhoneRegular).WithMessage(i18n.T("PhoneSpecInvalid"));
            RuleFor(x => x.SmsCode).NotEmpty().WithMessage(i18n.T("SmsRequired"));
        });
        When(login => !string.IsNullOrEmpty(login.Password), () =>
        {
            RuleFor(x => x.Password).MinimumLength(8).Equal(x => x.ConfirmPassword)
            .WithMessage(i18n.T("Password not equal ConfirmPassword"));
        });
    }
}
