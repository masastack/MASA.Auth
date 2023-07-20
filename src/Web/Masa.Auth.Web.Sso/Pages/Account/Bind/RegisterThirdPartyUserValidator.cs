// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Bind;

public class RegisterThirdPartyUserValidator : AbstractValidator<RegisterThirdPartyUserModel>
{
    public RegisterThirdPartyUserValidator(I18n i18n)
    {
        RuleFor(user => user.PhoneNumber)
            .NotEmpty()
            .WithMessage(i18n.T("PhoneRequired"))
            .Matches(LoginOptions.PhoneRegular)
            .WithMessage(i18n.T("PhoneSpecInvalid"));

        RuleFor(user => user.SmsCode)
            .NotEmpty()
            .WithMessage(i18n.T("SmsRequired"));
    }
}
