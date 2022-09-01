﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class ResetUserPasswordValidator : AbstractValidator<ResetUserPasswordDto>
{
    public ResetUserPasswordValidator()
    {
        RuleFor(staff => staff.Password).Required().AuthPassword();
    }
}

