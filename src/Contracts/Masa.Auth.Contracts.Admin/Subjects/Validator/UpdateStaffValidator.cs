// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateStaffValidator : AbstractValidator<UpdateStaffDto>
{
    public UpdateStaffValidator()
    {
        RuleFor(staff => staff.JobNumber).Required();
        RuleFor(user => user.DisplayName).Required().ChineseLetterNumber().MaxLength(20);
        RuleFor(user => user.Name).ChineseLetter().MaxLength(20);
        RuleFor(user => user.PhoneNumber).Required().Phone();
        RuleFor(user => user.Email).Email();
        RuleFor(user => user.IdCard).IdCard();
        RuleFor(user => user.Position).ChineseLetterNumber().MaxLength(20);
    }
}

