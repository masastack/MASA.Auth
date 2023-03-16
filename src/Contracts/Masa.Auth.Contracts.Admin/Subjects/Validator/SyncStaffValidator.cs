// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class SyncStaffValidator : MasaAbstractValidator<SyncStaffDto>
{
    public SyncStaffValidator()
    {
        RuleFor(staff => staff.DisplayName).NotEmpty()
                                           .WithMessage("NickName is required")
                                           .ChineseLetterNumber()
                                           .MinimumLength(2)
                                           .MaximumLength(50)
                                           .OverridePropertyName("NickName");
        WhenNotNullOrEmpty(staff => staff.Name, IRuleBuilder => IRuleBuilder.ChineseLetterNumber().MinimumLength(2).MaximumLength(50));
        RuleFor(staff => staff.PhoneNumber).Required().Phone();
        WhenNotNullOrEmpty(staff => staff.Email, IRuleBuilder => IRuleBuilder.Email());
        WhenNotNullOrEmpty(staff => staff.IdCard, IRuleBuilder => IRuleBuilder.IdCard());
        WhenNotNullOrEmpty(staff => staff.Position, IRuleBuilder => IRuleBuilder.ChineseLetterNumber().MinimumLength(2).MaximumLength(50));
        RuleFor(staff => staff.JobNumber).Required().LetterNumber().MinimumLength(4).MaximumLength(12);
    }
}

