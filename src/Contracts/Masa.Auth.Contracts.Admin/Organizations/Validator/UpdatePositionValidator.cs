// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations.Validator;

public class UpdatePositionValidator : AbstractValidator<UpdatePositionDto>
{
    public UpdatePositionValidator()
    {
        RuleFor(p => p.Id).NotEmpty();
        RuleFor(p => p.Name).Required().MinLength(2).MaxLength(20);
    }
}
