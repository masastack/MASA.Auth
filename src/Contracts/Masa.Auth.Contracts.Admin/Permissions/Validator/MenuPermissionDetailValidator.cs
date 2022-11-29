// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions.Validator;

public class MenuPermissionDetailValidator : AbstractValidator<MenuPermissionDetailDto>
{
    public MenuPermissionDetailValidator()
    {
        Include(new PermissionDetailValidator<MenuPermissionDetailDto>());
    }
}
