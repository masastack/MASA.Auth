// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso.Commands;

public class UpdateCustomLoginCommandValidator : AbstractValidator<UpdateCustomLoginCommand>
{
    public UpdateCustomLoginCommandValidator() => RuleFor(cmd => cmd.CustomLogin).SetValidator(new UpdateCustomLoginValidator());
}
