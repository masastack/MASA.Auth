// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso.Validator;

public class UpdateApiScopeValidator : AbstractValidator<UpdateApiScopeDto>
{
    public UpdateApiScopeValidator()
    {
        RuleFor(apiScope => apiScope.Name).Required().MaxLength(20);
        RuleFor(apiScope => apiScope.DisplayName).Required().MaxLength(50);
        RuleFor(apiScope => apiScope.Description).MaxLength(100);
    }
}

