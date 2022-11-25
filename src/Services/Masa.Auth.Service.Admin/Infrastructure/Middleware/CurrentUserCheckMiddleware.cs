// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Middleware;

public class CurrentUserCheckMiddleware : IMiddleware, IScopedDependency
{
    readonly IUserContext _userContext;
    readonly IUserRepository _userRepository;

    public CurrentUserCheckMiddleware(IUserContext userContext, IUserRepository userRepository)
    {
        _userContext = userContext;
        _userRepository = userRepository;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var userId = _userContext.GetUserId<Guid>();
        if (userId == Guid.Empty)
        {
            return next(context);
        }
        if (!_userRepository.Any(u => u.Id == userId))
        {
            throw new NoUserException(NoUserException.Code);
        }
        return next(context);
    }
}
