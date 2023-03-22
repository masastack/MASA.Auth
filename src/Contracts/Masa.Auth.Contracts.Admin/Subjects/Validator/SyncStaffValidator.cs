// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class SyncStaffValidator : MasaAbstractValidator<SyncStaffDto>
{
    public SyncStaffValidator()
    {
        RuleFor(staff => staff.DisplayName)
            .Required().WithMessage("NickNameBlock.Required")
            .ChineseLetterNumber().WithMessage("NickNameBlock.ChineseLetterNumber")
            .MaximumLength(50).WithMessage("NickNameBlock.MaxLength")
            .MinimumLength(2).WithMessage("NickNameBlock.MinLength")
            .WithName("NickName");
        RuleFor(staff => staff.PhoneNumber).Required().Phone();
        RuleFor(staff => staff.JobNumber).Required().LetterNumber().MinimumLength(4).MaximumLength(12);
        WhenNotEmpty(staff => staff.Name, r => r.ChineseLetterNumber().MinimumLength(2).MaximumLength(50));
        WhenNotEmpty(staff => staff.Email, r => r.Email());
        WhenNotEmpty(staff => staff.IdCard, r => r.IdCard());
        WhenNotEmpty(staff => staff.Position, r => r.ChineseLetterNumber().MinimumLength(2).MaximumLength(50));
    }
}
