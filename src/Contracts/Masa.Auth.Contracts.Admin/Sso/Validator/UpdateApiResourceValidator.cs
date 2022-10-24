// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class UpdateApiResourceValidator : AbstractValidator<UpdateApiResourceDto>
{
    public UpdateApiResourceValidator()
    {
        RuleFor(apiResource => apiResource.Name).Required().MinLength(2).MaxLength(50);
        RuleFor(apiResource => apiResource.DisplayName).Required().MinLength(2).MaxLength(50);
        RuleFor(apiResource => apiResource.Description).MaxLength(255);
    }
}

