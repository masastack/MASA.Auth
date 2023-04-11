﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateUserPasswordCommandValidator : MasaAbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator(PasswordValidator passwordValidator)
    {
        WhenNotEmpty(command => command.User.OldPassword, r => r.SetValidator(passwordValidator));
        RuleFor(command => command.User.NewPassword).SetValidator(passwordValidator);
        RuleFor(command => command.User.NewPassword)
                .Required()
                .NotEqual(command => command.User.OldPassword)
                .WithMessage("Old and new passwords cannot be the same");
    }
}
