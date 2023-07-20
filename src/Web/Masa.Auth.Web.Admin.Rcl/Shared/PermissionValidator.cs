// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public class PermissionValidator : IPermissionValidator
{
    private readonly IUserContext _userContext;
    private readonly PermissionService _permissionService;
    private readonly IMultiEnvironmentMasaStackConfig _multiEnvironmentMasaStackConfig;

    private string _environment = "";

    public PermissionValidator(
        IUserContext userContext,
        AuthCaller authCaller,
        IMultiEnvironmentMasaStackConfig multiEnvironmentMasaStackConfig,
        IMultiEnvironmentUserContext multiEnvironmentUserContext)
    {
        _userContext = userContext;
        _permissionService = authCaller.PermissionService;
        _multiEnvironmentMasaStackConfig = multiEnvironmentMasaStackConfig;

        _environment = multiEnvironmentUserContext.Environment ?? _environment;
    }

    public bool Validate(string code, ClaimsPrincipal user)
    {
        var userId = _userContext.GetUserId<Guid>();
        var codes = new List<string>();
        //TODO change Async and use Redis
        Task.Run(async () =>
        {
            codes = await _permissionService.GetElementPermissionsAsync(userId, _multiEnvironmentMasaStackConfig.SetEnvironment(_environment).GetWebId(MasaStackProject.Auth));
        }).Wait();
        return codes.Contains(code);
    }
}
