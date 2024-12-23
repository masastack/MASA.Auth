// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class AddApiScopeValidator : AbstractValidator<AddApiScopeDto>
{
    public AddApiScopeValidator()
    {
        RuleFor(apiScope => apiScope.Name).Required().MinimumLength(2).MaximumLength(50);
        RuleFor(apiScope => apiScope.DisplayName).Required().MinimumLength(2).MaximumLength(50);
        RuleFor(apiScope => apiScope.Description).MaximumLength(255);
    }
}

