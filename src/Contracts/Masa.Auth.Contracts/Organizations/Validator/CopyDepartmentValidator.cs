// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations.Validator;

public class CopyDepartmentValidator : AbstractValidator<CopyDepartmentDto>
{
    public CopyDepartmentValidator()
    {
        RuleFor(d => d.Name).Required();
        RuleFor(d => d.SourceId).Required();
    }
}
