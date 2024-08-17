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
            var subjectId = context.Subject.Claims.FirstOrDefault(c => c.Type == IdentityClaimConsts.USER_ID);
            if (subjectId != null && Guid.TryParse(subjectId.Value, out var userId))
            {
                var claimValues = await _authClient.UserService.GetClaimValuesAsync(userId);
                foreach (var claimValue in claimValues)
                {
                    AddOrUpdateClaims(context.IssuedClaims, new Claim(claimValue.Key, claimValue.Value), x => x.Type == claimValue.Key);
                }

                await AddThirdPartyClaimsAsync(context, userId);
            }
        }
    }

    public Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
        return Task.CompletedTask;
    }

    private async Task AddThirdPartyClaimsAsync(ProfileDataRequestContext context, Guid userId)
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
                        AddOrUpdateClaims(context.IssuedClaims, new Claim(item.Key, item.Value), x => x.Type == item.Key);
                    }
                }
            }
        }
    }

    private void AddOrUpdateClaims(List<Claim> list, Claim item, Predicate<Claim> match)
    {
        int index = list.FindIndex(match);
        if (index != -1)
        {
            list[index] = item;
        }
        else
        {
            list.Add(item); 
        }
    }
}
