// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class AddStaffCommandValidator : AbstractValidator<AddStaffCommand>
{
    public AddStaffCommandValidator()
    {
        RuleFor(command => command.Staff.JobNumber).Required().MaximumLength(20);
        RuleFor(command => command.Staff.DisplayName).MaximumLength(50);
        When(command => !string.IsNullOrEmpty(command.Staff.Name), () => RuleFor(command => command.Staff.Name).ChineseLetter().MaximumLength(20));
        RuleFor(command => command.Staff.PhoneNumber).Required().Phone();
        RuleFor(command => command.Staff.Email).Email();
        When(command => !string.IsNullOrEmpty(command.Staff.IdCard), () => RuleFor(command => command.Staff.IdCard).IdCard());
        When(command => !string.IsNullOrEmpty(command.Staff.CompanyName), () => RuleFor(command => command.Staff.CompanyName).ChineseLetterNumber().MaximumLength(50));
        When(command => !string.IsNullOrEmpty(command.Staff.Position), () => RuleFor(command => command.Staff.Position).ChineseLetterNumber().MaximumLength(20));
        RuleFor(command => command.Staff.Password).AuthPassword();
    }
}
