// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class SyncStaffValidator : AbstractValidator<SyncStaffDto>
{
    public SyncStaffValidator()
    {
        RuleFor(staff => staff.DisplayName).Required().ChineseLetterNumber().MinLength(2).MaxLength(50);
        RuleFor(staff => staff.Name).ChineseLetterNumber().MinLength(2).MaxLength(50);
        RuleFor(staff => staff.PhoneNumber).Required().Phone();
        RuleFor(staff => staff.Email).Email();
        RuleFor(staff => staff.IdCard).IdCard();
        RuleFor(staff => staff.Position).ChineseLetterNumber().MinLength(2).MaxLength(50);
        RuleFor(staff => staff.Password).Required().AuthPassword();
        RuleFor(staff => staff.JobNumber).Required().LetterNumber().MinLength(4).MaxLength(12);         
    }
}

