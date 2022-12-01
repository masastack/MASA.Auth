// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddTeamValidator : AbstractValidator<AddTeamDto>
{
    public AddTeamValidator()
    {
        RuleFor(team => team.Name).NotEmpty().WithMessage("Team name cannot be empty").MinLength(2).MaxLength(50);
    }
}
