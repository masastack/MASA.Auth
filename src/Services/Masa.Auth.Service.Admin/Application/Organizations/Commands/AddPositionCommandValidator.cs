// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations.Commands;

public class AddPositionCommandValidator : AbstractValidator<AddPositionCommand>
{
    public AddPositionCommandValidator()
    {
        RuleFor(command => command.Position).SetValidator(new AddPositionValidator());
    }
}
