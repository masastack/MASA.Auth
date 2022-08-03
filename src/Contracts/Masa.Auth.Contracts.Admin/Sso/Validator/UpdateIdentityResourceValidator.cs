// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class UpdateIdentityResourceValidator : AbstractValidator<UpdateIdentityResourceDto>
{
    public UpdateIdentityResourceValidator()
    {
        RuleFor(identityResource => identityResource.DisplayName).Required().MaxLength(50);
        RuleFor(identityResource => identityResource.Description).MaxLength(100);
    }
}

