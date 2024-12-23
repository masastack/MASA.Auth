// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddApiResourceValidator : AbstractValidator<AddApiResourceDto>
{
    public AddApiResourceValidator()
    {
        RuleFor(apiResource => apiResource.Name).Required().MinimumLength(2).MaximumLength(50);
        RuleFor(apiResource => apiResource.DisplayName).Required().MinimumLength(2).MaximumLength(50);
        RuleFor(apiResource => apiResource.Description).MaximumLength(255);
    }
}

