// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class UpdateUserClaimValidator : AbstractValidator<UpdateUserClaimDto>
{
    public UpdateUserClaimValidator()
    {
        RuleFor(UserClaim => UserClaim.Name).Required();
    }
}

