// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class UpdateUserPhoneNumberCommandValidator : AbstractValidator<UpdateUserPhoneNumberCommand>
{
    public UpdateUserPhoneNumberCommandValidator()
    {
        RuleFor(command => command.User.PhoneNumber).Required().Phone(CultureInfo.CurrentUICulture.Name);
    }
}
