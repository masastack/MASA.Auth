// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateUserPasswordCommandValidator : MasaAbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator(PasswordValidator passwordValidator)
    {
        RuleFor(command => command.User.NewPassword).SetValidator(passwordValidator);
        RuleFor(command => command.User.NewPassword)
                .Required()
                .NotEqual(command => command.User.OldPassword)
                .WithMessage(I18n.T("PasswordSame"));
    }
}
