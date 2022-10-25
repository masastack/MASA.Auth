// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions.Validator;

public class AddRoleValidator : AbstractValidator<AddRoleDto>
{
    public AddRoleValidator()
    {
        RuleFor(role => role.Name).Required().ChineseLetterNumber().MinLength(2).MaxLength(50);
        RuleFor(role => role.Code).Required().MinLength(2).MaxLength(150);
        RuleFor(role => role.Description).MaxLength(50);
        RuleFor(role => role.Limit).GreaterThanOrEqualTo(0);
    }
}

