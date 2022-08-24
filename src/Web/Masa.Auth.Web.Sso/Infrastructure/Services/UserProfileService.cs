// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Services;

public class UserProfileService : IProfileService
{
    public Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var claims = context.Subject.Claims.ToList();
        context.IssuedClaims.AddRange(claims);
        return Task.CompletedTask;
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub");
        context.IsActive = true;
        return Task.CompletedTask;
    }
}
