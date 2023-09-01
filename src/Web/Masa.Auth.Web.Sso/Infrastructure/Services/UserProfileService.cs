// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Services;

public class UserProfileService : IProfileService
{
    readonly IAuthClient _authClient;

    public UserProfileService(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var claims = context.Subject.Claims.ToList();
        context.IssuedClaims.AddRange(claims);
        //ClaimsProviderAccessToken
        //if (context.Caller == "ClaimsProviderIdentityToken" || context.Caller == "UserInfoEndpoint")
        {
            var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == "sub");
            if (subjectId != null && Guid.TryParse(subjectId.Value, out var userId))
            {
                var claimValues = await _authClient.UserService.GetClaimValuesAsync(userId);
                foreach (var claimValue in claimValues)
                {
                    context.IssuedClaims.TryAdd(new Claim(claimValue.Key, claimValue.Value));
                }
            }
        }
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }
}
