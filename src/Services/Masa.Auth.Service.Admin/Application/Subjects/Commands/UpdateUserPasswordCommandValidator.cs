// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateUserPasswordCommandValidator : AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator()
    {
        RuleFor(command => command.User.OldPassword).AuthPassword();

        RuleFor(command => command.User.NewPassword).Required().AuthPassword();

        RuleFor(command => command.User.NewPassword)
                .NotEmpty()
                .NotEqual(command => command.User.OldPassword).WithMessage("Old and new passwords cannot be the same");
    }
}
