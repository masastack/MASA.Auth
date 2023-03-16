// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateStaffCommandValidator : MasaAbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(command => command.Staff.JobNumber).Required().MaximumLength(20);
        RuleFor(command => command.Staff.DisplayName).Required().MaximumLength(50);
        RuleFor(command => command.Staff.PhoneNumber).Required().Phone();
        WhenNotEmpty(command => command.Staff.Email, r => r.Email());
        WhenNotEmpty(command => command.Staff.IdCard, r => r.IdCard());
        WhenNotEmpty(command => command.Staff.Name, r => r.ChineseLetter().MaximumLength(20));
        WhenNotEmpty(command => command.Staff.CompanyName, r => r.ChineseLetter().MaximumLength(50));
        WhenNotEmpty(command => command.Staff.Position, r => r.ChineseLetterNumber().MaximumLength(20));
    }
}
