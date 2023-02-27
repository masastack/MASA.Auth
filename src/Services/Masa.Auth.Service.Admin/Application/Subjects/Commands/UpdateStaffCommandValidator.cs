// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateStaffCommandValidator : AbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(command => command.Staff.JobNumber).Required().MaximumLength(20);
        RuleFor(command => command.Staff.DisplayName).Required().MaximumLength(50);
        RuleFor(command => command.Staff.Name).ChineseLetter().MaximumLength(20);
        RuleFor(command => command.Staff.PhoneNumber).Required().Phone(CultureInfo.CurrentUICulture.Name);
        RuleFor(command => command.Staff.Email).Email();
        RuleFor(command => command.Staff.IdCard).IdCard(CultureInfo.CurrentUICulture.Name);
        RuleFor(command => command.Staff.CompanyName).ChineseLetter().MaximumLength(50);
        RuleFor(command => command.Staff.Position).ChineseLetterNumber().MaximumLength(20);
    }
}
