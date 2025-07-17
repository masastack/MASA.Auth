// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class ResourceOwnerPasswordValidator : BaseGrantValidator, IResourceOwnerPasswordValidator
{
    public ResourceOwnerPasswordValidator(IAuthClient authClient, ILogger<ResourceOwnerPasswordValidator> logger)
        : base(authClient, logger)
    {
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        try
        {
            var user = await _authClient.UserService.ValidateAccountAsync(new ValidateAccountModel
            {
                Account = context.UserName,
                Password = context.Password,
                Environment = context.Request.Raw.Get(nameof(IEnvironmentModel.Environment)) ?? ""
            });

            context.Result = new GrantValidationResult(
                 subject: user!.Id.ToString(),
                 authenticationMethod: OidcConstants.AuthenticationMethods.Password);

            // 记录Token获取的操作日志（包含客户端信息）
            await RecordTokenOperationLogAsync(user, "用户Token获取：使用密码模式获取访问Token", context.Request.Client?.ClientId, nameof(ResourceOwnerPasswordValidator));
        }
        catch (Exception ex)
        {
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                ex.Message);
        }
    }
}
