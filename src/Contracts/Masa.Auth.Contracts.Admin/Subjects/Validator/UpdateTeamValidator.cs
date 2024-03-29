﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateTeamValidator : AbstractValidator<UpdateTeamDto>
{
    public UpdateTeamValidator()
    {
        RuleFor(team => team.Name).Required().WithMessage("Team name cannot be empty").MinimumLength(2).MaximumLength(50);
    }
}
