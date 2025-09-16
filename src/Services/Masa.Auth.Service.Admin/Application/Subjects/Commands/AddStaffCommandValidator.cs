// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class AddStaffCommandValidator : MasaAbstractValidator<AddStaffCommand>
{
    public AddStaffCommandValidator(PasswordValidator passwordValidator, PhoneNumberValidator phoneValidator)
    {
        //TODO
        //和AddStaffValidator中存在大量重复代码，后续优化
        //RuleFor(command => command.Staff).SetValidator(addStaffValidator);

        RuleFor(command => command.Staff.JobNumber).Required().MinimumLength(4).MaximumLength(12);
        RuleFor(command => command.Staff.PhoneNumber).Required().SetValidator(phoneValidator);
        RuleFor(command => command.Staff.DisplayName).MaximumLength(50);
        WhenNotEmpty(command => command.Staff.Password, r => r.SetValidator(passwordValidator));
        WhenNotEmpty(command => command.Staff.Email, r => r.Email());
        WhenNotEmpty(command => command.Staff.Name, r => r.ChineseLetter().MaximumLength(20));
        WhenNotEmpty(command => command.Staff.IdCard, r => r.IdCard());
        WhenNotEmpty(command => command.Staff.CompanyName, r => r.ChineseLetter().MaximumLength(50));
        WhenNotEmpty(command => command.Staff.Position, r => r.ChineseLetter().MaximumLength(20));
    }
}
