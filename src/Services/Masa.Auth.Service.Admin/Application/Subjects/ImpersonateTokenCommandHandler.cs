// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityModel;

namespace Masa.Auth.Service.Admin.Application.Subjects;

/// <summary>
/// 模拟登录代理（RFC 8693 token-exchange）：以调用管理员的 access_token 作为 actor_token，
/// 凭配置的机密客户端凭据向 SSO /connect/token 换取被模拟用户的短命 access_token
/// （sub=目标用户、act=操作者、≤900s、无 refresh_token）。
/// WASM 前端不能持有 client_secret（token-exchange 只接受机密客户端），必须由此服务端代理；
/// 权限在两端各校验一次——这里按调用者校验 api.user.impersonate，SSO 侧校验 actor_token 持同一权限。
/// </summary>
public class ImpersonateTokenCommandHandler
{
    readonly IUserContext _userContext;
    readonly IEventBus _eventBus;
    readonly IHttpClientFactory _httpClientFactory;
    readonly IMasaStackConfig _masaStackConfig;
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IOperationLogRepository _operationLogRepository;
    readonly ImpersonationClientOptions _options;
    readonly ILogger<ImpersonateTokenCommandHandler> _logger;

    public ImpersonateTokenCommandHandler(
        IUserContext userContext,
        IEventBus eventBus,
        IHttpClientFactory httpClientFactory,
        IMasaStackConfig masaStackConfig,
        IHttpContextAccessor httpContextAccessor,
        IOperationLogRepository operationLogRepository,
        IOptions<ImpersonationClientOptions> options,
        ILogger<ImpersonateTokenCommandHandler> logger)
    {
        _userContext = userContext;
        _eventBus = eventBus;
        _httpClientFactory = httpClientFactory;
        _masaStackConfig = masaStackConfig;
        _httpContextAccessor = httpContextAccessor;
        _operationLogRepository = operationLogRepository;
        _options = options.Value;
        _logger = logger;
    }

    [EventHandler]
    public async Task ImpersonateTokenAsync(ImpersonateTokenCommand command)
    {
        var actorUserId = _userContext.GetUserId<Guid>();
        if (actorUserId == default)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
        }

        if (command.UserId == actorUserId)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IMPERSONATION_SELF_NOT_ALLOWED);
        }

        if (string.IsNullOrWhiteSpace(_options.ClientId) || string.IsNullOrWhiteSpace(_options.ClientSecret))
        {
            _logger.LogWarning(
                "Impersonation proxy is not configured: JSON config object 'Impersonation' (ClientId/ClientSecret) is missing under the 'auth-service' app in DCC");
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IMPERSONATION_CLIENT_NOT_CONFIGURED);
        }

        if (!await IsAuthorizedAsync(actorUserId))
        {
            _logger.LogWarning(
                "Impersonation rejected: actor {ActorUserId} lacks permission {PermissionCode}",
                actorUserId, ElementPermissionCodes.ApiUserImpersonate);
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.IMPERSONATION_NOT_AUTHORIZED);
        }

        var actorToken = GetCallerAccessToken();
        if (string.IsNullOrEmpty(actorToken))
        {
            // 经 RequireAuthorization 的端点正常一定带 Bearer；防御网关剥离 Authorization 头的部署
            _logger.LogWarning("Impersonation rejected: caller {ActorUserId} has no bearer token", actorUserId);
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
        }

        var output = await ExchangeAsync(command.UserId, actorUserId, actorToken);

        await _operationLogRepository.AddDefaultAsync(
            OperationTypes.Login,
            $"模拟登录：管理员{actorUserId}通过token-exchange模拟用户{command.UserId}获取访问Token",
            actorUserId);

        command.Result = output;
    }

    /// <summary>调用者是否持有模拟登录权限。fail-closed：查询异常按无权限处理。</summary>
    public virtual async Task<bool> IsAuthorizedAsync(Guid actorUserId)
    {
        try
        {
            var query = new UserAuthorizedDomainEvent(
                _masaStackConfig.GetServiceId(MasaStackProject.Auth),
                ElementPermissionCodes.ApiUserImpersonate,
                actorUserId);
            await _eventBus.PublishAsync(query);
            return query.Authorized;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Impersonation permission check failed, denying (fail-closed). userId:{UserId}, code:{Code}",
                actorUserId, ElementPermissionCodes.ApiUserImpersonate);
            return false;
        }
    }

    /// <summary>取当前请求的 Bearer access_token（作为 actor_token 原样转发给 SSO）。</summary>
    public virtual string? GetCallerAccessToken()
    {
        var authorization = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();
        return authorization?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true
            ? authorization["Bearer ".Length..].Trim()
            : null;
    }

    /// <summary>
    /// 调 SSO /connect/token 执行 token-exchange。HTTP 细节与业务判定隔离在此，便于单测替换。
    /// </summary>
    public virtual async Task<ImpersonateTokenOutput> ExchangeAsync(Guid targetUserId, Guid actorUserId, string actorToken)
    {
        var client = _httpClientFactory.CreateClient("SsoAddr");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            [OidcConstants.TokenRequest.GrantType] = GrantType.TOKEN_EXCHANGE,
            [OidcConstants.TokenRequest.ClientId] = _options.ClientId!,
            [OidcConstants.TokenRequest.ClientSecret] = _options.ClientSecret!,
            [OidcConstants.TokenRequest.ActorToken] = actorToken,
            [OidcConstants.TokenRequest.ActorTokenType] = OidcConstants.TokenTypeIdentifiers.AccessToken,
            [OidcConstants.TokenRequest.SubjectToken] = targetUserId.ToString(),
            [OidcConstants.TokenRequest.SubjectTokenType] = ImpersonationConsts.SubjectTokenTypeUserId,
            [OidcConstants.TokenRequest.Scope] = ResolveScope(_options.Scope)
        });

        using var response = await client.PostAsync("/connect/token", content);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // SSO 的 error_description 经脱敏且已本地化，优先透传给管理员
            var (error, description) = TryParseError(body);
            _logger.LogWarning(
                "Impersonation token-exchange rejected by SSO. httpStatus:{Status}, error:{Error}, target:{TargetUserId}, actor:{ActorUserId}",
                (int)response.StatusCode, error ?? "<unparseable>", targetUserId, actorUserId);
            throw new UserFriendlyException(
                errorCode: UserFriendlyExceptionCodes.IMPERSONATION_TOKEN_EXCHANGE_FAILED,
                description ?? error ?? body);
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;
            var accessToken = root.TryGetProperty("access_token", out var token) ? token.GetString() : null;
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new UserFriendlyException(
                    errorCode: UserFriendlyExceptionCodes.IMPERSONATION_TOKEN_EXCHANGE_FAILED);
            }

            var expiresIn = root.TryGetProperty("expires_in", out var expires) && expires.ValueKind == JsonValueKind.Number
                ? expires.GetInt32()
                : 0;

            return new ImpersonateTokenOutput
            {
                AccessToken = accessToken,
                ExpiresIn = expiresIn
            };
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex,
                "Impersonation token-exchange got unparseable success response. target:{TargetUserId}, actor:{ActorUserId}",
                targetUserId, actorUserId);
            throw new UserFriendlyException(
                errorCode: UserFriendlyExceptionCodes.IMPERSONATION_TOKEN_EXCHANGE_FAILED);
        }
    }

    /// <summary>
    /// 取配置的 scope：规整空白、去重；未配置则用默认值。token-exchange 要求 scope 必传
    /// （省略时 IdentityServer 会展开成客户端全部 scope，可能带上 offline_access 而被拒绝），所以这里始终显式给出。
    /// </summary>
    static string ResolveScope(string? configured)
    {
        var scopes = (configured ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.Ordinal);
        var resolved = string.Join(' ', scopes);
        return resolved.Length == 0 ? ImpersonationClientOptions.DefaultScope : resolved;
    }

    static (string? Error, string? Description) TryParseError(string body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;
            var error = root.TryGetProperty("error", out var errorElement) ? errorElement.GetString() : null;
            var description = root.TryGetProperty("error_description", out var descriptionElement)
                ? descriptionElement.GetString()
                : null;
            return (error, description);
        }
        catch (JsonException)
        {
            return (null, null);
        }
    }
}
