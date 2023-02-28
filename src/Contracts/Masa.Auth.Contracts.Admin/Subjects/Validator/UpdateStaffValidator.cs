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
        RuleFor(staff => staff.Name).ChineseLetterNumber().MinimumLength(2).MaximumLength(50);
        RuleFor(staff => staff.PhoneNumber).Required().Phone(CultureInfo.CurrentUICulture.Name);
        RuleFor(staff => staff.Email).Email();
        RuleFor(staff => staff.IdCard).IdCard(CultureInfo.CurrentUICulture.Name);
        RuleFor(staff => staff.Address.Address).MinimumLength(8).MaximumLength(100);
        RuleFor(staff => staff.CompanyName).ChineseLetter().MinimumLength(50);
        RuleFor(staff => staff.Position).ChineseLetterNumber().MinimumLength(2).MaximumLength(50);
        RuleFor(staff => staff.Avatar).Required().Url();
    }
}

