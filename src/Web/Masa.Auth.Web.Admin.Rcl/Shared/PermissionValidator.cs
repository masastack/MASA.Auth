// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Shared;

public class PermissionValidator : IPermissionValidator
{
    protected PermissionCodesCache Cache { get; }

    public PermissionValidator(IClientScopeServiceProviderAccessor clientScopeServiceProviderAccessor)
    {
        Cache = clientScopeServiceProviderAccessor.ServiceProvider.GetRequiredService<PermissionCodesCache>();
    }

    public bool Validate(string code, ClaimsPrincipal user)
    {
        return Cache.Codes.Contains(code);
    }
}