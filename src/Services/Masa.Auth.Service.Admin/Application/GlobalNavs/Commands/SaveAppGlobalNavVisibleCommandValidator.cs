// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.GlobalNavs.Commands;

public class SaveAppGlobalNavVisibleCommandValidator : MasaAbstractValidator<SaveAppGlobalNavVisibleCommand>
{
    public SaveAppGlobalNavVisibleCommandValidator()
    {
        RuleFor(command => command.visibleDto.AppId).Required();
        RuleFor(command => command.visibleDto.VisibleType).IsInEnum();
    }
}