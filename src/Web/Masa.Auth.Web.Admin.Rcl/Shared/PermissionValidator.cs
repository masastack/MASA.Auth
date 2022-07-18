// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public class PermissionValidator : IPermissionValidator
{
    readonly IUserContext _userContext;

    public PermissionValidator(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public bool Validate(string code, ClaimsPrincipal user)
    {
        var userId = _userContext.GetUserId<Guid>();
        //todo 校验权限,可以在登录系统后 获取一次存储在redis
        return true;
    }
}
