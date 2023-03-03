// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateStaffValidator : AbstractValidator<UpdateStaffDto>
{
    public UpdateStaffValidator()
    {
        RuleFor(staff => staff.JobNumber).Required().LetterNumber().MinimumLength(4).MaximumLength(12);
        RuleFor(staff => staff.DisplayName)
            .NotEmpty().WithMessage("NickName is required")
            .ChineseLetterNumber().MinimumLength(2).MaximumLength(50).OverridePropertyName("NickName");
        When(staff => !string.IsNullOrEmpty(staff.Name), () => RuleFor(staff => staff.Name).ChineseLetter().MinimumLength(2).MaximumLength(20));
        RuleFor(staff => staff.PhoneNumber).Required().Phone();
        RuleFor(staff => staff.Email).Email();
        When(staff => !string.IsNullOrEmpty(staff.IdCard), () => RuleFor(staff => staff.IdCard).IdCard());
        When(staff => !string.IsNullOrEmpty(staff.Address?.Address), () => RuleFor(staff => staff.IdCard).MinimumLength(8).MaximumLength(100));
        RuleFor(staff => staff.CompanyName).ChineseLetter().MaximumLength(50);
        When(staff => !string.IsNullOrEmpty(staff.Position), () => RuleFor(staff => staff.Position).ChineseLetterNumber().MaximumLength(20));
        RuleFor(staff => staff.Avatar).Required().Url();
    }
}

