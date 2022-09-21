// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validation;

public class PhoneCodeGrantValidator : IExtensionGrantValidator
{
    IAuthClient _authClient;

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.PHONE_CODE;

    public PhoneCodeGrantValidator(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        string? phoneNumber = context.Request.Raw["PhoneNumber"];
        string? code = context.Request.Raw["Code"];
        if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(code))
            throw new UserFriendlyException("must provider phone number and msg code");

        var user = await _authClient.UserService.LoginByPhoneNumberAsync(new LoginByPhoneNumberModel 
        {
            PhoneNumber = phoneNumber,
            Code = code
        });
        context.Result = new GrantValidationResult(user.Id.ToString(), "sms", GetUserClaims(user.DisplayName));
    }

    private Claim[] GetUserClaims(string name)
    {
        return new Claim[]
        {
            new Claim("username", name)
        };
    }
}
