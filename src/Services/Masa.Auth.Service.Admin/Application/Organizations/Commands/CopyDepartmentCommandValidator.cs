﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations.Commands;

public class CopyDepartmentCommandValidator : AbstractValidator<CopyDepartmentCommand>
{
    public CopyDepartmentCommandValidator()
    {
        RuleFor(command => command.CopyDepartmentDto).SetValidator(new CopyDepartmentValidator());
    }
}
