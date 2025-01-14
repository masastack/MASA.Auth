// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public class PermissionCodesCache : ISingletonDependency
{
    public List<string> Codes { get; private set; } = new();

    private readonly PermissionService _permissionService;
    private readonly IMultiEnvironmentMasaStackConfig _multiEnvironmentMasaStackConfig;
    private readonly IUserContext _userContext;
    private string _environment = "";

    public PermissionCodesCache(IClientScopeServiceProviderAccessor serviceProviderAccessor)
    {
        _permissionService = serviceProviderAccessor.ServiceProvider.GetRequiredService<AuthCaller>().PermissionService;
        _multiEnvironmentMasaStackConfig = serviceProviderAccessor.ServiceProvider.GetRequiredService<IMultiEnvironmentMasaStackConfig>();
        _userContext = serviceProviderAccessor.ServiceProvider.GetRequiredService<IUserContext>();
        _environment = serviceProviderAccessor.ServiceProvider.GetRequiredService<IMultiEnvironmentUserContext>().Environment ?? _environment;
    }

    public virtual async Task InitializeAsync()
    {
        var userId = _userContext.GetUserId<Guid>();

        if (userId != Guid.Empty)
        {
            Codes = await _permissionService.GetElementPermissionsAsync(userId, _multiEnvironmentMasaStackConfig.SetEnvironment(_environment).GetWebId(MasaStackProject.Auth));
        }
    }
}
