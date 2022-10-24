// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddIdentityResourceValidator : AbstractValidator<AddIdentityResourceDto>
{
    public AddIdentityResourceValidator()
    {
        RuleFor(identityResource => identityResource.Name).Required().MinLength(2).MaxLength(50);
        RuleFor(identityResource => identityResource.DisplayName).Required().MinLength(2).MaxLength(50);
        RuleFor(identityResource => identityResource.Description).MaxLength(255);
    }
}

