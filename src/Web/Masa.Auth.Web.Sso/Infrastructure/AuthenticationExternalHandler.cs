// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class AuthenticationExternalHandler : IAuthenticationExternalHandler
{
    readonly IAuthClient _authClient;
    readonly IIdentityServerInteractionService _interaction;
    readonly IEventService _events;
    readonly IHttpContextAccessor _contextAccessor;
    readonly ILogger<AuthenticationExternalHandler> _logger;

    public AuthenticationExternalHandler(
        IAuthClient authClient,
        IIdentityServerInteractionService interaction,
        IEventService events,
        IHttpContextAccessor contextAccessor,
        ILogger<AuthenticationExternalHandler> logger)
    {
        _authClient = authClient;
        _interaction = interaction;
        _events = events;
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

    public async Task<bool> OnHandleAuthenticateAfterAsync(AuthenticateResult result)
    {
        var scheme = result.Properties?.Items?["scheme"] ?? throw new UserFriendlyException("Unknown third party");
        var identity = IdentityProvider.GetIdentity(scheme, result.Principal ?? throw new UserFriendlyException("Authenticate failed"));
        result.Properties.Items.TryGetValue("environment", out var environment);
        environment ??= "development";
        var httpContext = _contextAccessor.HttpContext ?? throw new UserFriendlyException("Internal exception, please contact the administrator");
        var userModel = await _authClient.UserService.GetThirdPartyUserAsync(new GetThirdPartyUserModel
        {
            ThridPartyIdentity = identity.Subject,
            Environment = environment
        });
        if (userModel is not null)
        {
            ProcessLoginCallback(result, out var additionalLocalClaims, out var localSignInProps);
            additionalLocalClaims.Add(new Claim(IdentityClaimConsts.ENVIRONMENT, environment));
            additionalLocalClaims.Add(new Claim(IdentityClaimConsts.ACCOUNT, userModel.Account));
            additionalLocalClaims.Add(new Claim(IdentityClaimConsts.ROLES, JsonSerializer.Serialize(userModel.Roles.Select(role => role.Code))));
            additionalLocalClaims.Add(new Claim(IdentityClaimConsts.CURRENT_TEAM, (userModel.CurrentTeamId ?? Guid.Empty).ToString()));
            additionalLocalClaims.Add(new Claim(IdentityClaimConsts.STAFF, (userModel.StaffId ?? Guid.Empty).ToString()));
            additionalLocalClaims.Add(new Claim(IdentityClaimConsts.PHONE_NUMBER, userModel.PhoneNumber ?? ""));
            additionalLocalClaims.Add(new Claim(IdentityClaimConsts.EMAIL, userModel.Email ?? ""));
            var isuser = new IdentityServerUser(userModel.Id.ToString())
            {
                DisplayName = userModel.DisplayName,

                IdentityProvider = scheme,
                AdditionalClaims = additionalLocalClaims
            };
            await httpContext.SignInAsync(isuser, localSignInProps);
            await httpContext.SingOutExternalAsync();

            var returnUrl = result.Properties?.Items?["returnUrl"] ?? "~/";
            // check if external login is in the context of an OIDC request
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            await _events.RaiseAsync(new UserLoginSuccessEvent(userModel.Name, userModel.Id.ToString(), userModel.DisplayName, clientId: context?.Client.ClientId));

            // 记录第三方登录的操作日志（包含客户端信息）
            await RecordThirdPartyLoginOperationLogAsync(userModel, scheme, context?.Client.ClientId);

            return true;
        }
        else
        {
            httpContext.Response.Redirect($"/account/user/bind");
            return false;
        }
    }

    /// <summary>
    /// 记录第三方登录操作日志（包含客户端信息）
    /// </summary>
    private async Task RecordThirdPartyLoginOperationLogAsync(UserModel user, string scheme, string? clientId)
    {
        try
        {
            var operationLogModel = new AddOperationLogModel(
                user.Id,
                user.DisplayName,
                OperationTypes.Login,
                DateTime.UtcNow,
                $"用户登录：使用{scheme}第三方登录",
                clientId);

            await _authClient.OperationLogService.AddLogAsync(operationLogModel);
        }
        catch (Exception ex)
        {
            // 记录日志失败不应该影响登录流程，只记录错误
            _logger.LogError(ex, "Failed to record third-party login operation log for user {UserId} using {Scheme}", user.Id, scheme);
        }
    }

    static void ProcessLoginCallback(AuthenticateResult externalResult, out List<Claim> localClaims, out AuthenticationProperties localSignInProps)
    {
        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        localClaims = new List<Claim>();
        localSignInProps = new AuthenticationProperties();

        // if the external provider issued an id_token, we'll keep it for signout
        var id_token = externalResult.Properties?.GetTokenValue("id_token");
        if (id_token != null)
        {
            localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
        }
    }
}
