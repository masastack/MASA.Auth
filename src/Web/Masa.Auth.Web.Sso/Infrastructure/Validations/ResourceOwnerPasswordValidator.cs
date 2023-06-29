// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    readonly IAuthClient _authClient;

    public ResourceOwnerPasswordValidator(IAuthClient authClient)
    {
        _authClient = authClient;
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
                 authenticationMethod: OidcConstants.AuthenticationMethods.Password,
                 claims: user.GetUserClaims());
        }
        catch (Exception ex)
        {
            context.Result = new GrantValidationResult(
                TokenRequestErrors.InvalidGrant,
                ex.Message);
        }
    }
}
