// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public class PermissionValidator : IPermissionValidator
{
    readonly IUserContext _userContext;
    readonly GlobalConfig _globalConfig;
    readonly PermissionService _permissionService;

    public PermissionValidator(IUserContext userContext, GlobalConfig globalConfig, AuthCaller authCaller)
    {
        _userContext = userContext;
        _globalConfig = globalConfig;
        _permissionService = authCaller.PermissionService;
    }

    public bool Validate(string code, ClaimsPrincipal user)
    {
        var userId = _userContext.GetUserId<Guid>();
        if (_globalConfig.ElementPermissions == null)
        {
            Task.Run(async () =>
            {
                _globalConfig.ElementPermissions = await _permissionService.GetElementPermissionsAsync(userId, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID);
            }).Wait();
        }
        return _globalConfig.ElementPermissions!.Contains(code);
    }
}
