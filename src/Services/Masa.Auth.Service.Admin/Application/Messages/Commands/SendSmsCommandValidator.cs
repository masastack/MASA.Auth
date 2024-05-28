// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class SendSmsCommandValidator : MasaAbstractValidator<SendSmsCommand>
{
    public SendSmsCommandValidator(PhoneNumberValidator phoneValidator)
    {
        WhenNotEmpty(command => command.Model.PhoneNumber, r => r.SetValidator(phoneValidator));
    }
}
