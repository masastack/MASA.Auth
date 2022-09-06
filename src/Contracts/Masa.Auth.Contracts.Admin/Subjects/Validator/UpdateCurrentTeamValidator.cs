// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

internal class UpdateCurrentTeamValidator : AbstractValidator<UpdateCurrentTeamDto>
{
    public UpdateCurrentTeamValidator()
    {
        RuleFor(staff => staff.UserId).Required();
        RuleFor(staff => staff.TeamId).Required();
    }
}
