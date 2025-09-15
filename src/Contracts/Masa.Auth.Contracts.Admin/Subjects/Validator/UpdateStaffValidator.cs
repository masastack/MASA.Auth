// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class UpdateStaffValidator : MasaAbstractValidator<UpdateStaffDto>
{
    public UpdateStaffValidator(PhoneNumberValidator phoneValidator)
    {
        RuleFor(staff => staff.JobNumber).Required().LetterNumber().MinimumLength(4).MaximumLength(12);
        RuleFor(staff => staff.DisplayName)
            .Required().WithMessage("NickNameBlock.Required")
            .ChineseLetterNumber().WithMessage("NickNameBlock.ChineseLetterNumber")
            .MaximumLength(50).WithMessage("NickNameBlock.MaxLength")
            .MinimumLength(2).WithMessage("NickNameBlock.MinLength")
            .WithName("NickName"); ;
        RuleFor(staff => staff.PhoneNumber).Required().SetValidator(phoneValidator);
        RuleFor(staff => staff.Avatar).Required().Url();
        WhenNotEmpty(l => l.CompanyName, r => r.ChineseLetter().MaximumLength(50));
        WhenNotEmpty(staff => staff.Email, r => r.Email());
        WhenNotEmpty(staff => staff.Name, r => r.ChineseLetter().MinimumLength(2).MaximumLength(50));
        WhenNotEmpty(staff => staff.IdCard, r => r.IdCard());
        WhenNotEmpty(staff => staff.Address.Address, r => r.MinimumLength(8).MaximumLength(100));
        WhenNotEmpty(staff => staff.CompanyName, r => r.ChineseLetter().MaximumLength(50));
        WhenNotEmpty(staff => staff.Position, r => r.ChineseLetter().MinimumLength(2).MaximumLength(50));
    }
}
