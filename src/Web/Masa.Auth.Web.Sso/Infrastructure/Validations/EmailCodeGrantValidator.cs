// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class EmailCodeGrantValidator : BaseGrantValidator, IExtensionGrantValidator
{
    public string GrantType { get; } = "email_code";

    public EmailCodeGrantValidator(IAuthClient authClient, ILogger<EmailCodeGrantValidator> logger)
        : base(authClient, logger)
    {
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        try
        {
            var email = context.Request.Raw["Email"];
            var code = context.Request.Raw["Code"];
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
                throw new UserFriendlyException(errorCode: SsoErrorCodes.MUST_PROVIDER_EMAIL_AND_CODE);

            var user = await _authClient.UserService.LoginByEmailAsync(new LoginByEmailModel
            {
                Email = email,
                Code = code
            });
            if (user != null)
            {
                context.Result = new GrantValidationResult(user.Id.ToString(), "email_code");

                // Record token acquisition operation log (including client information)
                await RecordTokenOperationLogAsync(user, $"用户Token获取：使用邮箱{email}验证码获取访问Token", context.Request.Client?.ClientId, nameof(EmailCodeGrantValidator));
            }
            else
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.INVALID_CUSTOM_CREDENTIAL);
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
