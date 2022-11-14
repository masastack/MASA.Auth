// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public class PermissionValidator : IPermissionValidator
{
    readonly IUserContext _userContext;
    readonly PermissionService _permissionService;

    public PermissionValidator(IUserContext userContext, AuthCaller authCaller)
    {
        _userContext = userContext;
        _permissionService = authCaller.PermissionService;
    }

    public bool Validate(string code, ClaimsPrincipal user)
    {
        var userId = _userContext.GetUserId<Guid>();
        var codes = new List<string>();
        //todo change Async and use redis
        Task.Run(async () =>
        {
            codes = await _permissionService.GetElementPermissionsAsync(userId, MasaStackConsts.AUTH_SYSTEM_WEB_APP_ID);
        }).Wait();
        return codes.Contains(code);
    }
}
