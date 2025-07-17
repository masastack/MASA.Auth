// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class PhoneCodeGrantValidator : BaseGrantValidator, IExtensionGrantValidator
{
    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.PHONE_CODE;

    public PhoneCodeGrantValidator(IAuthClient authClient, ILogger<PhoneCodeGrantValidator> logger)
        : base(authClient, logger)
    {
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        try
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
                context.Result = new GrantValidationResult(user.Id.ToString(), "sms");

                // 记录Token获取的操作日志（包含客户端信息）
                await RecordTokenOperationLogAsync(user, $"用户Token获取：使用手机号{phoneNumber}验证码获取访问Token", context.Request.Client?.ClientId, nameof(PhoneCodeGrantValidator));
            }
            else
            {
                context.Result = new GrantValidationResult(
                    TokenRequestErrors.InvalidGrant,
                    "invalid custom credential");
            }
        }
        catch (Exception ex)
        {
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                ex.Message);
        }
    }
}
