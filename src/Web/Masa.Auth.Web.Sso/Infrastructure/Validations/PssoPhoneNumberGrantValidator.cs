// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class PssoPhoneNumberGrantValidator : IExtensionGrantValidator
{
    IAuthClient _authClient;
    LocalLoginByPhoneNumberAgent _localLoginByPhoneNumber;

    public string GrantType { get; } = "psso_phone";

    public PssoPhoneNumberGrantValidator(IAuthClient authClient, LocalLoginByPhoneNumberAgent localLoginByPhoneNumber)
    {
        _authClient = authClient;
        _localLoginByPhoneNumber = localLoginByPhoneNumber;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var phoneNumber = context.Request.Raw["PhoneNumber"];
        var spToken = context.Request.Raw["SpToken"];
        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(spToken))
        {
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = "Must provide phone number and spToken",
                ErrorDescription = "Must provide phone number and spToken"
            };
            return;
        }

        var (success, errorMsg) = await _localLoginByPhoneNumber.VerifyPhoneWithTokenAsync(phoneNumber, spToken);
        if (success)
        {
            var user = await _authClient.UserService.GetByPhoneNumberAsync(phoneNumber);
            if (user is null)
            {
                context.Result = new GrantValidationResult
                {
                    IsError = true,
                    Error = $"User {phoneNumber} does not exist",
                    ErrorDescription = errorMsg
                };
            }
            else
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

                context.Result = new GrantValidationResult(user.Id.ToString(), "local", claims);
            }
        }
        else
            context.Result = new GrantValidationResult
            {
                IsError = true,
                Error = errorMsg,
                ErrorDescription = errorMsg
            };
    }
}
