// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddThirdPartyIdpValidator : AbstractValidator<AddThirdPartyIdpDto>
{
    public AddThirdPartyIdpValidator()
    {
        RuleFor(thirdPartyIdp => thirdPartyIdp.Name).Required().ChineseLetterNumber().MinimumLength(2).MaximumLength(50);
        RuleFor(thirdPartyIdp => thirdPartyIdp.DisplayName).Required().ChineseLetterNumber().MinimumLength(2).MaximumLength(50);
        RuleFor(thirdPartyIdp => thirdPartyIdp.ClientId).Required().LetterNumber().MinimumLength(2).MaximumLength(50);
        RuleFor(thirdPartyIdp => thirdPartyIdp.ClientSecret).Required().MinimumLength(2).MaximumLength(255);
        RuleFor(thirdPartyIdp => thirdPartyIdp.Icon).Required().Url();
        RuleFor(thirdPartyIdp => thirdPartyIdp.AuthorizationEndpoint).Required().Url();
        RuleFor(thirdPartyIdp => thirdPartyIdp.TokenEndpoint).Required().Url();
        RuleFor(thirdPartyIdp => thirdPartyIdp.UserInformationEndpoint).Required().Url();
        RuleFor(thirdPartyIdp => thirdPartyIdp.CallbackPath).Required();
    }
}

