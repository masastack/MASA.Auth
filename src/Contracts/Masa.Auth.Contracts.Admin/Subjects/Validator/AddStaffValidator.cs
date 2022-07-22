// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects.Validator;

public class AddStaffValidator : AbstractValidator<AddStaffDto>
{
    public AddStaffValidator()
    {
        RuleFor(staff => staff.JobNumber).Required();
        RuleFor(staff => staff.DisplayName).Required().ChineseLetterNumber().MaxLength(20);
        RuleFor(staff => staff.Name).ChineseLetter().MaxLength(20);
        RuleFor(staff => staff.PhoneNumber).Required().Phone();
        RuleFor(staff => staff.Email).Email();
        RuleFor(staff => staff.IdCard).IdCard();
        RuleFor(staff => staff.CompanyName).ChineseLetter().MaxLength(50);
        RuleFor(staff => staff.Position).ChineseLetterNumber().MaxLength(20);
        RuleFor(staff => staff.Account).Required().ChineseLetterNumber();
        RuleFor(staff => staff.Password).Required()
                                      .Matches(@"^\S*(?=\S{8,})(?=\S*\d)(?=\S*[A-Za-z])\S*$")
                                      .WithMessage("Password must contain numbers and letter, and not less than 8 digits")
                                      .MaxLength(30);
    }
}

