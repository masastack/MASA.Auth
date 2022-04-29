// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations.Validator;

public class UpsertPositionValidator : AbstractValidator<UpsertPositionDto>
{
    public UpsertPositionValidator()
    {
        RuleFor(p => p.Name).Required();
    }
}
