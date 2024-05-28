// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Infrastructure.Phone;

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class PhoneNumberValidator : MasaAbstractValidator<string?>
{
    public PhoneNumberValidator(PhoneHelper phoneHelper)
    {
        RuleFor(phone => phone).Custom(phoneHelper.ValidatePhone);
    }
}
