// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class AddTeamCommandValidator : AbstractValidator<AddTeamCommand>
{
    public AddTeamCommandValidator()
    {
        RuleFor(team => team).Required().WithMessage($"Parameter error");
        RuleFor(team => team.AddTeamDto.Name).Must(name => !string.IsNullOrEmpty(name) && name.Length <= 20)
            .WithMessage("Team Name can`t null and length must be less than 20.");
        RuleFor(team => team.AddTeamDto.Description).Must(description => description.Length <= 255)
            .WithMessage("Team Description length must be less than 255.");
    }
}
