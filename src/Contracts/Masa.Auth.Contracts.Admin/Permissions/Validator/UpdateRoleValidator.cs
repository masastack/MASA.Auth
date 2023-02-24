// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions.Validator;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    public UpdateRoleValidator()
    {
        RuleFor(role => role.Name).Required().ChineseLetterNumber().MinimumLength(2).MaximumLength(50);
        RuleFor(role => role.Code).Required().MinimumLength(2).MaximumLength(150);
        RuleFor(role => role.Description).MaximumLength(50);
        RuleFor(role => role.Limit).GreaterThanOrEqualTo(0);
    }
}

