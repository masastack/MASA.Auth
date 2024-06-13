// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class LocalPhoneNumberGrantValidator : IExtensionGrantValidator
{
    IAuthClient _authClient;
    LocalLoginByPhoneNumberAgent _localLoginByPhoneNumber;

    public string GrantType { get; } = "local_phone";

    public LocalPhoneNumberGrantValidator(IAuthClient authClient, LocalLoginByPhoneNumberAgent localLoginByPhoneNumber)
    {
        _authClient = authClient;
        _localLoginByPhoneNumber = localLoginByPhoneNumber;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        try
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
                    context.Result = new GrantValidationResult(user.Id.ToString(), "local");
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
        catch (Exception ex)
        {
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                ex.Message);
        }
    }
}
