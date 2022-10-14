// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class RegisterInputModelValidator : AbstractValidator<RegisterInputModel>
{
    public RegisterInputModelValidator(I18n i18N)
    {
        When(login => login.EmailRegister, () =>
        {
            RuleFor(x => x.Email).NotEmpty().Matches(LoginOptions.EmailRegular);
            RuleFor(x => x.EmailCode).NotEmpty().Must(x => x >= LoginOptions.CodeMinimum && x <= LoginOptions.CodeMaximum);
        }).Otherwise(() =>
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().Matches(LoginOptions.PhoneRegular);
            RuleFor(x => x.SmsCode).NotEmpty().Must(x => x >= LoginOptions.CodeMinimum && x <= LoginOptions.CodeMaximum);
        });
        When(login => !string.IsNullOrEmpty(login.Password), () =>
        {
            RuleFor(x => x.Password).MinimumLength(8).Equal(x => x.ConfirmPassword)
            .WithMessage(i18N.T("Password not equal ConfirmPassword"));
        });
    }
}
