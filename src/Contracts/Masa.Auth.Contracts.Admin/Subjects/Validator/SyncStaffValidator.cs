// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class SyncStaffValidator : AbstractValidator<SyncStaffDto>
{
    public SyncStaffValidator()
    {
        RuleFor(staff => staff.DisplayName).ChineseLetter().MinLength(1).MaxLength(20);
        RuleFor(staff => staff.Name).Required().ChineseLetter().MaxLength(20);
        RuleFor(staff => staff.PhoneNumber).Phone();
        RuleFor(staff => staff.Email).Email();
        RuleFor(staff => staff.IdCard).IdCard();
        RuleFor(staff => staff.Position).ChineseLetterNumber().MaxLength(20);
        RuleFor(staff => staff.Account).Required().ChineseLetterNumber();
        RuleFor(staff => staff.Password).Required().LetterNumber();
        RuleFor(staff => staff.JobNumber).Required();
        RuleFor(staff => staff.Position).MaxLength(50);
    }
}

