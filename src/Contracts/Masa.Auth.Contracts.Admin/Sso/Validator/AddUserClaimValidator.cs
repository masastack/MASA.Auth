// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddUserClaimValidator : AbstractValidator<AddUserClaimDto>
{
    public AddUserClaimValidator()
    {
        RuleFor(UserClaim => UserClaim.Name).Required().MinLength(2).MaxLength(50);
        RuleFor(UserClaim => UserClaim.Description).Required().MinLength(2).MaxLength(255);
    }
}

