﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects.Commands;

public class VerifyMsgCodeCommandValidator : AbstractValidator<VerifyMsgCodeCommand>
{
    public VerifyMsgCodeCommandValidator()
    {
        RuleFor(command => command.Model.Code).Required();
        RuleFor(command => command.Model.PhoneNumber).Required();
        RuleFor(command => command.Model.SendMsgCodeType).Required();
    }
}
