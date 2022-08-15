// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddThirdPartyUserValidator : AbstractValidator<AddThirdPartyUserDto>
{
    public AddThirdPartyUserValidator()
    {
        RuleFor(tpu => tpu.User).SetValidator(new AddUserValidator());
        RuleFor(tpu => tpu.ThridPartyIdentity).Required();
    }
}

