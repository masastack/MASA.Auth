// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class LoginByPhoneNumberCommandValidator : AbstractValidator<LoginByPhoneNumberCommand>
{
    public LoginByPhoneNumberCommandValidator(PhoneNumberValidator phoneValidator)
    {
        RuleFor(command => command.Model.PhoneNumber).Required().SetValidator(phoneValidator);
        RuleFor(command => command.Model.Code).Required();
    }
}
