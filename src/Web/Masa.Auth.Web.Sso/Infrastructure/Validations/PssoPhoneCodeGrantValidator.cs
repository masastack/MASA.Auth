// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class PssoPhoneCodeGrantValidator : IExtensionGrantValidator
{
    IAuthClient _authClient;

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.PSSO_PHONE_CODE;

    public PssoPhoneCodeGrantValidator(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var phoneNumber = context.Request.Raw["PhoneNumber"];
        var code = context.Request.Raw["Code"];
        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(code))
            throw new UserFriendlyException("must provider phone number and msg code");

        var user = await _authClient.UserService.LoginByPhoneNumberAsync(new LoginByPhoneNumberModel
        {
            PhoneNumber = phoneNumber,
            Code = code
        });
        if (user != null)
        {
            var authUser = await _authClient.UserService.GetThirdPartyUserByUserIdAsync(new GetThirdPartyUserByUserIdModel
            {
                Scheme = "Psso",
                UserId = user.Id
            });

            var claims = new List<Claim>();
            if (authUser != null)
            {
                foreach (var item in authUser.ClaimData)
                {
                    claims.Add(new Claim(item.Key, item.Value));
                }
            }

            context.Result = new GrantValidationResult(user.Id.ToString(), "sms", claims);
        }
        else
        {
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                "invalid custom credential");
        }

    }
}