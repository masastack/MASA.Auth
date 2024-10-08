﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class ImpersonationGrantValidator : IExtensionGrantValidator
{
    IAuthClient _authClient;
    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.IMPERSONATION;

    public ImpersonationGrantValidator(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        try
        {
            var impersonationToken = context.Request.Raw["impersonationToken"];
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
                claims.Add(new Claim(IdentityClaimConsts.IMPERSONATOR_USER_ID, cacheItem.ImpersonatorUserId.ToString()));
            }

            context.Result = new GrantValidationResult(cacheItem.TargetUserId.ToString(), "impersonation", claims);
        }
        catch (Exception ex)
        {
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                ex.Message);
        }
    }
}
