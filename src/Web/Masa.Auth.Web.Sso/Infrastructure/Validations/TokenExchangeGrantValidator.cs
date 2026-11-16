// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

/// <summary>
/// RFC 8693 Token Exchange：管理员凭 <c>actor_token</c> 模拟目标用户；校验 <c>api.user.impersonate</c> 权限、短 TTL、禁止 offline_access 与链式模拟。
/// 不比较目标与 actor 的权限高低：该权限等同可成为任何人，只授予完全可信的人员。
/// </summary>
public class TokenExchangeGrantValidator : BaseGrantValidator, IExtensionGrantValidator
{
    readonly ITokenValidator _tokenValidator;
    readonly ActorPermissionClient _actorPermissionClient;

    /// <summary>模拟 access token 最长有效期（秒）。</summary>
    public const int MaxAccessTokenLifetimeSeconds = 900;

    /// <summary>
    /// 禁止作为 <c>actor_token</c> 的 <c>amr</c>：无密码的 <c>wia</c>，避免先断言管理员再换身份。
    /// 模拟态 token 不在此列：它们都带 <c>act</c>，由链式模拟检查拒绝（模拟态只认 <c>act</c>）。
    /// </summary>
    public static readonly IReadOnlyList<string> DeniedActorAuthenticationMethods = new[]
    {
        OidcConstants.AuthenticationMethods.WindowsIntegratedAuthentication
    };

    /// <summary>自定义 <c>subject_token_type</c>：目标用户 id 字符串（授权来自 <c>actor_token</c>）。</summary>
    public const string SubjectTokenTypeUserId = "urn:masa:params:oauth:token-type:user-id";

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.TOKEN_EXCHANGE;

    public TokenExchangeGrantValidator(
        IAuthClient authClient,
        ITokenValidator tokenValidator,
        ActorPermissionClient actorPermissionClient,
        ILogger<TokenExchangeGrantValidator> logger)
        : base(authClient, logger)
    {
        _tokenValidator = tokenValidator;
        _actorPermissionClient = actorPermissionClient;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var client = context.Request.Client;

        try
        {
            // 仅机密客户端：公开 client_id 不能单独承担「谁在发起模拟」这一层保证
            if (!client.RequireClientSecret)
            {
                _logger.LogWarning(
                    "Impersonation rejected: client {ClientId} is a public client (RequireClientSecret=false)",
                    client.ClientId);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.IMPERSONATION_DISABLED);
            }

            // 禁止 offline_access，否则短 TTL 可被 refresh 绕过
            if (RequestsOfflineAccess(context))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.IMPERSONATION_OFFLINE_ACCESS_NOT_ALLOWED);
            }

            // 硬上限写在 Request.AccessTokenLifetime（exp 据此计算，非 CustomResponse）
            context.Request.AccessTokenLifetime = Math.Min(
                context.Request.AccessTokenLifetime, MaxAccessTokenLifetimeSeconds);

            var actorToken = context.Request.Raw[OidcConstants.TokenRequest.ActorToken];
            if (string.IsNullOrWhiteSpace(actorToken))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.MUST_PROVIDER_ACTOR_TOKEN);
            }

            // RFC 8693 §2.1：有 actor_token 则 actor_token_type 必须为 access_token
            var actorTokenType = context.Request.Raw[OidcConstants.TokenRequest.ActorTokenType];
            if (!string.Equals(actorTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken, StringComparison.Ordinal))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.INVALID_TOKEN_TYPE);
            }

            // 本 grant 只签 access_token；出现其它 requested_token_type 则拒绝
            var requestedTokenType = context.Request.Raw[OidcConstants.TokenRequest.RequestedTokenType];
            if (!string.IsNullOrEmpty(requestedTokenType)
                && !string.Equals(requestedTokenType, OidcConstants.TokenTypeIdentifiers.AccessToken, StringComparison.Ordinal))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.INVALID_TOKEN_TYPE);
            }

            // 本地验签 actor access_token
            var actor = await _tokenValidator.ValidateAccessTokenAsync(actorToken, null);
            if (actor.IsError || actor.Claims is null)
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.INVALID_ACTOR_TOKEN);
            }

            var actorUserIdText = actor.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)?.Value;
            if (!Guid.TryParse(actorUserIdText, out var actorUserId))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.INVALID_ACTOR_TOKEN);
            }

            // 禁止链式模拟：actor 自身含 act 则拒绝
            if (actor.Claims.Any(c => c.Type == JwtClaimTypes.Actor))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.IMPERSONATION_CHAIN_NOT_ALLOWED);
            }

            // 拒绝 wia 等禁止的 actor amr
            var deniedMethods = ImpersonationPolicy.FindDeniedAuthenticationMethods(
                actor.Claims.Where(c => c.Type == JwtClaimTypes.AuthenticationMethod).Select(c => c.Value),
                DeniedActorAuthenticationMethods);
            if (deniedMethods.Count > 0)
            {
                _logger.LogWarning(
                    "Impersonation rejected: actor {ActorUserId} token was issued via {Methods} (clientId:{ClientId})",
                    actorUserId, string.Join(',', deniedMethods), client.ClientId);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.IMPERSONATION_NOT_AUTHORIZED);
            }

            // JWT 不自反映账号状态；IProfileService 不拦禁用用户，须回 Admin 复核 actor
            var actorUser = await _actorPermissionClient.GetUserAsync(actorUserId, actorToken);
            if (actorUser is null || actorUser.IsDeleted || !actorUser.Enabled)
            {
                _logger.LogWarning(
                    "Impersonation rejected: actor {ActorUserId} is missing, deleted or disabled (clientId:{ClientId})",
                    actorUserId, client.ClientId);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.INVALID_ACTOR_TOKEN);
            }

            // 须显式 api.user.impersonate；/connect/token 无当前用户，权限查询带 actor userId
            var authorized = await _actorPermissionClient.IsAuthorizedAsync(
                actorUserId, ElementPermissionCodes.ApiUserImpersonate);
            if (!authorized)
            {
                _logger.LogWarning(
                    "Impersonation rejected: actor {ActorUserId} lacks permission {PermissionCode} (clientId:{ClientId})",
                    actorUserId, ElementPermissionCodes.ApiUserImpersonate, client.ClientId);
                throw new UserFriendlyException(errorCode: SsoErrorCodes.IMPERSONATION_NOT_AUTHORIZED);
            }

            var subjectToken = context.Request.Raw[OidcConstants.TokenRequest.SubjectToken];
            if (!Guid.TryParse(subjectToken, out var targetUserId))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.MUST_PROVIDER_SUBJECT_TOKEN);
            }

            // subject_token_type 须为自定义 user-id（授权来自 actor_token，类型错即拒）
            var subjectTokenType = context.Request.Raw[OidcConstants.TokenRequest.SubjectTokenType];
            if (!string.Equals(subjectTokenType, SubjectTokenTypeUserId, StringComparison.Ordinal))
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.INVALID_TOKEN_TYPE);
            }

            if (targetUserId == actorUserId)
            {
                // 禁止 actor 与 target 相同
                throw new UserFriendlyException(errorCode: SsoErrorCodes.MUST_PROVIDER_SUBJECT_TOKEN);
            }

            var targetUser = await _actorPermissionClient.GetUserAsync(targetUserId, actorToken);
            // 目标须在职；统一错误码避免账号探测
            if (targetUser is null || targetUser.IsDeleted || !targetUser.Enabled)
            {
                throw new UserFriendlyException(errorCode: SsoErrorCodes.IMPERSONATION_TARGET_NOT_FOUND);
            }

            // 不做防提权比较：api.user.impersonate 等同可成为任何人，只授予完全可信的人员（见 docs/sso-impersonation.md 2.2）

            // RFC 8693 §4.1 act；须 valueType=Json，否则 JWT 里 act 会变成转义字符串而非对象
            var act = ImpersonationPolicy.BuildActClaim(actorUserId);

            var claims = new List<Claim>
            {
                new(JwtClaimTypes.Actor, act, IdentityServerConstants.ClaimValueTypes.Json)
            };

            // IS4 要求必须传 authenticationMethod（它会写成 token 里唯一一条 amr）。目标用户并未亲自认证，
            // 故不用 pwd/ldap，免得只看 amr 的下游把它当成"用户输过密码"；判定模拟态以 act 为准，勿用 amr
            context.Result = new GrantValidationResult(
                targetUserId.ToString(),
                "impersonation",
                claims);

            // RFC 8693 §2.2.1 issued_token_type
            context.Result.CustomResponse ??= new Dictionary<string, object>();
            context.Result.CustomResponse[OidcConstants.TokenResponse.IssuedTokenType] = OidcConstants.TokenTypeIdentifiers.AccessToken;

            await RecordTokenOperationLogAsync(
                targetUser,
                $"用户Token获取：管理员{actorUserId}通过客户端{client.ClientId}模拟用户{targetUserId}获取访问Token",
                client.ClientId,
                nameof(TokenExchangeGrantValidator));
        }
        catch (Exception ex)
        {
            // 业务拒 Warning、基础设施 Error；error_description 经 ToErrorDescription 脱敏
            _logger.Log(ex is UserFriendlyException ? LogLevel.Warning : LogLevel.Error, ex,
                "Extension grant {GrantType} rejected. clientId:{ClientId}, error:{ErrorMessage}",
                GrantType, client.ClientId, ex.Message);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, ToErrorDescription(ex));
        }
    }

    static bool RequestsOfflineAccess(ExtensionGrantValidationContext context)
        => ImpersonationPolicy.RequestsOfflineAccess(
            context.Request.RequestedScopes,
            context.Request.Raw["scope"]);
}