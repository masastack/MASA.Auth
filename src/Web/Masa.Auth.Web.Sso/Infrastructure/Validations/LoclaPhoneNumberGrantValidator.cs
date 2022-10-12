// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class LoclaPhoneNumberGrantValidator : IExtensionGrantValidator
{
    IAuthClient _authClient;
    LocalLoginByPhoneNumberAgent _localLoginByPhoneNumber;

    public string GrantType { get; } = "local_phone";

    public LoclaPhoneNumberGrantValidator(IAuthClient authClient, LocalLoginByPhoneNumberAgent localLoginByPhoneNumber)
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
            var user = await _authClient.UserService.FindByPhoneNumberAsync(phoneNumber);
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
                context.Result = new GrantValidationResult(user.Id.ToString(), "local", GetUserClaims(user.DisplayName));
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

    private Claim[] GetUserClaims(string name)
    {
        return new Claim[]
        {
            new Claim("username", name)
        };
    }
}
