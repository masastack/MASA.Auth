// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateStaffCommandValidator : AbstractValidator<UpdateStaffCommand>
{
    public UpdateStaffCommandValidator()
    {
        RuleFor(command => command.Staff.JobNumber).Required();
        RuleFor(command => command.Staff.DisplayName).Required().ChineseLetterNumber().MaxLength(20);
        RuleFor(command => command.Staff.Name).ChineseLetter().MaxLength(20);
        RuleFor(command => command.Staff.PhoneNumber).Phone();
        RuleFor(command => command.Staff.Email).Email();
        RuleFor(command => command.Staff.IdCard).IdCard();
        RuleFor(command => command.Staff.CompanyName).ChineseLetter().MaxLength(50);
        RuleFor(command => command.Staff.Position).ChineseLetterNumber().MaxLength(20);
    }
}
