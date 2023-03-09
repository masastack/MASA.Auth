// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateUserCommandValidator : MasaAbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(command => command.User.DisplayName).Required().MaximumLength(50);
        WhenNotNullOrEmpty(command => command.User.Name, roleBuilder => roleBuilder.ChineseLetterNumber().MaximumLength(20));
        RuleFor(command => command.User.PhoneNumber).Phone();
        WhenNotNullOrEmpty(command => command.User.Email, roleBuilder => roleBuilder.Email());
        WhenNotNullOrEmpty(command => command.User.IdCard, roleBuilder => roleBuilder.IdCard());
        WhenNotNullOrEmpty(command => command.User.CompanyName, roleBuilder => roleBuilder.ChineseLetterNumber().MaximumLength(50));
        WhenNotNullOrEmpty(command => command.User.Position, roleBuilder => roleBuilder.ChineseLetterNumber().MaximumLength(20));
    }
}
