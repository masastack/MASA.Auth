// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public class PermissionValidator : IPermissionValidator
{
    readonly IUserContext _userContext;
    readonly PermissionService _permissionService;
    readonly IMasaStackConfig _masaStackConfig;

    public PermissionValidator(IUserContext userContext, AuthCaller authCaller, IMasaStackConfig masaStackConfig)
    {
        _userContext = userContext;
        _permissionService = authCaller.PermissionService;
        _masaStackConfig = masaStackConfig;
    }

    public bool Validate(string code, ClaimsPrincipal user)
    {
        var userId = _userContext.GetUserId<Guid>();
        var codes = new List<string>();
        //todo change Async and use redis
        Task.Run(async () =>
        {
            codes = await _permissionService.GetElementPermissionsAsync(userId, _masaStackConfig.GetWebId(MasaStackConstant.AUTH));
        }).Wait();
        return codes.Contains(code);
    }
}
