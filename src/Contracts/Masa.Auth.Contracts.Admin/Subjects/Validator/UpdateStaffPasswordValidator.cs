// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateStaffPasswordValidator : AbstractValidator<UpdateStaffPasswordDto>
{
    public UpdateStaffPasswordValidator()
    {
        RuleFor(staff => staff.Password).Required()
                                      .Matches(@"^\S*(?=\S{6,})(?=\S*\d)(?=\S*[A-Za-z])\S*$")
                                      .WithMessage("Password must contain numbers and letter, and not less than 6 digits")
                                      .MaxLength(30);
    }
}

