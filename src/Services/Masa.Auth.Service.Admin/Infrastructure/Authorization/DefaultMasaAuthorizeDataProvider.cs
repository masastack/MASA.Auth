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

    public Task<string> GetAccountAsync()
    {
        var account = _userContext.UserName ?? "";
        return Task.FromResult(account);
    }

    public async Task<IEnumerable<string>> GetAllowCodeListAsync(string appId)
    {
        var userId = _userContext.GetUserId<Guid>();
        if (userId == Guid.Empty)
        {
            return Enumerable.Empty<string>();
        }
        var permissionQuery = new UserElementPermissionCodeQuery(appId, userId);
        await _eventBus.PublishAsync(permissionQuery);
        return permissionQuery.Result;
    }
}
