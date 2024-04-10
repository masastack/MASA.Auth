// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Services;

public class UserProfileService : IProfileService
{
    readonly IAuthClient _authClient;
    readonly IHttpContextAccessor _httpContextAccessor;

    public UserProfileService(IAuthClient authClient, IHttpContextAccessor httpContextAccessor)
    {
        _authClient = authClient;
        _httpContextAccessor = httpContextAccessor;
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
                var request = _httpContextAccessor.HttpContext?.Request;
                if (request != null)
                {
                    if (request.HasFormContentType && request.Form.TryGetValue("scheme", out var scheme))
                    {
                        var authUser = await _authClient.UserService.GetThirdPartyUserByUserIdAsync(new GetThirdPartyUserByUserIdModel
                        {
                            Scheme = scheme,
                            UserId = userId
                        });

                        if (authUser != null)
                        {
                            foreach (var item in authUser.ClaimData)
                            {
                                context.IssuedClaims.TryAdd(new Claim(item.Key, item.Value));
                            }
                        }
                    }
                }

                var claimValues = await _authClient.UserService.GetClaimValuesAsync(userId);
                foreach (var claimValue in claimValues)
                {
                    if (!context.IssuedClaims.Any(x=>x.Type == claimValue.Key))
                    {
                        context.IssuedClaims.TryAdd(new Claim(claimValue.Key, claimValue.Value));
                    }
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
