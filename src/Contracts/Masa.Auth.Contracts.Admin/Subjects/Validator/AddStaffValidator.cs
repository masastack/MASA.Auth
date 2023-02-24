// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddStaffValidator : AbstractValidator<AddStaffDto>
{
    public AddStaffValidator()
    {
        RuleFor(staff => staff.JobNumber).Required().LetterNumber().MinimumLength(4).MaximumLength(12);
        RuleFor(staff => staff.DisplayName)
            .NotEmpty().WithMessage("NickName is required")
            .ChineseLetterNumber().MinimumLength(2).MaximumLength(50).OverridePropertyName("NickName");
        RuleFor(staff => staff.Name).ChineseLetterNumber().MinimumLength(2).MaximumLength(50);
        RuleFor(staff => staff.PhoneNumber).Required().Phone();
        RuleFor(staff => staff.Email).EmailAddress();
        RuleFor(staff => staff.IdCard).IdCard();
        RuleFor(staff => staff.Address.Address).MinimumLength(8).MaximumLength(100);
        RuleFor(staff => staff.CompanyName).ChineseLetter().MaximumLength(50);
        RuleFor(staff => staff.Position).ChineseLetterNumber().MinimumLength(2).MaximumLength(50);
        RuleFor(staff => staff.Password).Required().AuthPassword();
        RuleFor(staff => staff.Avatar).Required().Url();
    }
}

