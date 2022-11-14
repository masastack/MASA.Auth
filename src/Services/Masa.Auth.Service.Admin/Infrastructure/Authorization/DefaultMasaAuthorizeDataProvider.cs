// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public class DefaultMasaAuthorizeDataProvider : IMasaAuthorizeDataProvider
{
    readonly IUserContext _userContext;
    readonly IEventBus _eventBus;

    public DefaultMasaAuthorizeDataProvider(IUserContext userContext, IEventBus eventBus)
    {
        _userContext = userContext;
        _eventBus = eventBus;
    }

    public Task<IEnumerable<string>> GetRolesAsync()
    {
        var roles = _userContext.GetUserRoles<string>();
        return Task.FromResult(roles);
    }

    public async Task<IEnumerable<string>> GetAllowCodesAsync(string appId)
    {
        var userId = _userContext.GetUserId<Guid>();
        if (userId == Guid.Empty)
        {
            return Enumerable.Empty<string>();
        }
        var permissionQuery = new UserApiPermissionCodeQuery(appId, userId);
        await _eventBus.PublishAsync(permissionQuery);
        return permissionQuery.Result;
    }
}
