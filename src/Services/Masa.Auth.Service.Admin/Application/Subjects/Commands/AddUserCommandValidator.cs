// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class AddUserCommandValidator : AbstractValidator<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        RuleFor(command => command.User.DisplayName).MaximumLength(50);
        RuleFor(command => command.User.Name).ChineseLetterNumber().MaximumLength(20);
        RuleFor(command => command.User.PhoneNumber).Phone(CultureInfo.CurrentUICulture.Name);
        RuleFor(command => command.User.Email).Email();
        RuleFor(command => command.User.IdCard).IdCard(CultureInfo.CurrentUICulture.Name);
        RuleFor(command => command.User.CompanyName).ChineseLetterNumber().MaximumLength(50);
        RuleFor(command => command.User.Position).ChineseLetterNumber().MaximumLength(20);
        RuleFor(command => command.User.Account).MaximumLength(50);
        RuleFor(command => command.User.Password).AuthPassword();
    }
}
