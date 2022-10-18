// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Bind;

public class RegisterThirdPartyUserValidator : AbstractValidator<RegisterThirdPartyUserModel>
{
    public RegisterThirdPartyUserValidator()
    {
        RuleFor(user => user.PhoneNumber)
            .NotEmpty()
            .WithMessage("phone number cannot be empty")
            .Matches(@"^\s{0}$|^((\+86)|(86))?(1[3-9][0-9])\d{8}$")
            .WithMessage("{PropertyName} format is incorrect");

        RuleFor(user => user.SmsCode)
            .NotEmpty()
            .WithMessage("captcha cannot be empty")
            .Length(6)
            .WithMessage("captcha length is six");
    }
}
