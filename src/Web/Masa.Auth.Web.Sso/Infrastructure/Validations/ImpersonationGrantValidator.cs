// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class ImpersonationGrantValidator : IExtensionGrantValidator
{
    IAuthClient _authClient;
    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.IMPERSONATION;

    const string IMPERSONATOR_USER_ID = "http://Lonsid.org/identity/claims/impersonatorUserId";

    public ImpersonationGrantValidator(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var impersonationToken = context.Request.Raw["impersonationToken"];
        var scheme = context.Request.Raw["scheme"] ?? string.Empty;
        var environment = context.Request.Raw["environment"] ?? string.Empty;
        if (string.IsNullOrEmpty(impersonationToken))
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = "Must provide impersonationToken",
                ErrorDescription = "Must provide impersonationToken"
            };
            return;
        }

        var input = new GetImpersonateInputModel
        {
            ImpersonationToken = impersonationToken,
            Environment = environment
        };

        var cacheItem = await _authClient.UserService.GetImpersonateAsync(input);
        if (cacheItem is null)
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = "Impersonated user does not exist",
                ErrorDescription = "Impersonated user does not exist",
            };
            return;
        }

        var claims = new List<Claim>();

        if (!cacheItem.IsBackToImpersonator)
        {
            claims.Add(new Claim(IMPERSONATOR_USER_ID, cacheItem.ImpersonatorUserId.ToString()));
        }

        if (!string.IsNullOrEmpty(scheme))
        {
            var authUser = await _authClient.UserService.GetThirdPartyUserByUserIdAsync(new GetThirdPartyUserByUserIdModel
            {
                Scheme = scheme,
                UserId = cacheItem.TargetUserId
            });

            if (authUser != null)
            {
                foreach (var item in authUser.ClaimData)
                {
                    claims.Add(new Claim(item.Key, item.Value));
                }
            }
        }

        context.Result = new GrantValidationResult(cacheItem.TargetUserId.ToString(), "impersonation", claims);
    }
}
