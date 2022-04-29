// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateStaffValidator : AbstractValidator<UpdateStaffDto>
{
    public UpdateStaffValidator()
    {
        RuleFor(staff => staff.User).SetValidator(new UpdateUserValidator());
        RuleFor(staff => staff.JobNumber).Required();
    }
}

