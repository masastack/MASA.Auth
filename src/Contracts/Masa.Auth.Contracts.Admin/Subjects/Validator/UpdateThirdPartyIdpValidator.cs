// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateThirdPartyIdpValidator : AbstractValidator<UpdateThirdPartyIdpDto>
{
    public UpdateThirdPartyIdpValidator()
    {
        RuleFor(thirdPartyIdp => thirdPartyIdp.DisplayName).ChineseLetterNumber().MinLength(4).MaxLength(50);
        RuleFor(staff => staff.ClientId).Required().LetterNumber().MinLength(4).MaxLength(50);
        RuleFor(staff => staff.ClientSecret).Required().MaxLength(255);
        RuleFor(staff => staff.Url).Required().Url().MaxLength(255);
        RuleFor(staff => staff.Icon).Required();
    }
}

