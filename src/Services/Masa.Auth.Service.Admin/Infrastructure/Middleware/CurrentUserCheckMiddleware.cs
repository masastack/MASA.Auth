// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class CurrentUserCheckMiddleware : IMiddleware, IScopedDependency
{
    readonly IUserContext _userContext;
    readonly IMultilevelCacheClient _multilevelCacheClient;

    public CurrentUserCheckMiddleware(IUserContext userContext, AuthClientMultilevelCacheProvider authClientMultilevelCacheProvider)
    {
        _userContext = userContext;
        _multilevelCacheClient = authClientMultilevelCacheProvider.GetMultilevelCacheClient();
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userId = _userContext.GetUserId<Guid>();
        var user = await _multilevelCacheClient.GetUserAsync(userId);
        if (user != null)
        {
            if (user.IsDeleted)
            {
                throw new UserStatusException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
            }
            if (!user.Enabled)
            {
                throw new UserStatusException(errorCode: UserFriendlyExceptionCodes.USER_FROZEN);
            }
        }

        await next(context);
    }
}
