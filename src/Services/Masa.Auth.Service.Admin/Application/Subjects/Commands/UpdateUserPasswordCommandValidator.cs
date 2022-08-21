// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateUserPasswordCommandValidator : AbstractValidator<UpdateUserPasswordCommand>
{
    public UpdateUserPasswordCommandValidator()
    {
        RuleFor(command => command.User.OldPassword).Required()
                              .Matches(BusinessConsts.PASSWORD_REGULAR)
                              .WithMessage("Password must contain numbers and letter, and not less than 6 digits")
                              .MaxLength(30);

        RuleFor(command => command.User.NewPassword).Required()
                      .Matches(BusinessConsts.PASSWORD_REGULAR)
                      .WithMessage("Password must contain numbers and letter, and not less than 6 digits")
                      .MaxLength(30);

        RuleFor(command => command.User.NewPassword)
                .NotEmpty()
                .NotEqual(command => command.User.OldPassword).WithMessage("Old and new passwords cannot be the same");
    }
}
