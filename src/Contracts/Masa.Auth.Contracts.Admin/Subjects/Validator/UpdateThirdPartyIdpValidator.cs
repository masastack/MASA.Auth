// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateThirdPartyIdpValidator : AbstractValidator<UpdateThirdPartyIdpDto>
{
    public UpdateThirdPartyIdpValidator()
    {
        RuleFor(staff => staff.ClientId).Required();
        RuleFor(staff => staff.ClientSecret).Required();
        RuleFor(staff => staff.Url).Required();
        //RuleFor(staff => staff.Icon).Required();
    }
}

