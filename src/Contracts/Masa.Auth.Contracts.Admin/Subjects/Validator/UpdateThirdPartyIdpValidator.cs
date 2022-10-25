// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateThirdPartyIdpValidator : AbstractValidator<UpdateThirdPartyIdpDto>
{
    public UpdateThirdPartyIdpValidator()
    {
        RuleFor(thirdPartyIdp => thirdPartyIdp.DisplayName).Required().ChineseLetterNumber().MinLength(2).MaxLength(50);
        RuleFor(thirdPartyIdp => thirdPartyIdp.ClientId).Required().LetterNumber().MinLength(2).MaxLength(50);
        RuleFor(thirdPartyIdp => thirdPartyIdp.ClientSecret).Required().MinLength(2).MaxLength(255);
        RuleFor(thirdPartyIdp => thirdPartyIdp.Icon).Required().Url();
        RuleFor(thirdPartyIdp => thirdPartyIdp.AuthorizationEndpoint).Required().Url();
        RuleFor(thirdPartyIdp => thirdPartyIdp.TokenEndpoint).Required().Url();
        RuleFor(thirdPartyIdp => thirdPartyIdp.UserInformationEndpoint).Required().Url();
        RuleFor(thirdPartyIdp => thirdPartyIdp.CallbackPath).Required();
    }
}

