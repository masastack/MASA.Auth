// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateUserBasicInfoCommandValidator : AbstractValidator<UpdateUserBasicInfoCommand>
{
    public UpdateUserBasicInfoCommandValidator()
    {
        RuleFor(command => command.User.DisplayName).Required().MaximumLength(20);
    }
}
