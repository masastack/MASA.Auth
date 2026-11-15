// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Duende.AspNetCore.Authentication.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Hybrid;

namespace Masa.Auth.Service.Admin.Infrastructure.Auth;

/// <summary>混合 Bearer 校验的 scheme 名。对外的默认 scheme 仍是 "Bearer"（PolicyScheme），
/// 内部按令牌形状转发到 <see cref="Jwt"/>（本地验签）或 <see cref="Introspection"/>（Reference 客户端 opaque 令牌）</summary>
public static class AuthSchemes
{
    public const string Bearer = "Bearer";
    public const string Jwt = "jwt";
    public const string Introspection = "introspection";
}

/// <summary>
/// 令牌形状分流（纯函数，测试锚定）：
/// - JWT 是 JWS 紧凑序列化（恰好三段、含两个 '.'）→ 本地验签；
/// - reference token 是 IS4 生成的 opaque 句柄（32 字节随机数的 base64url，43 个字符，无 '.'）→ introspection；
/// - 两者都不像的垃圾凭据 → 也走 jwt：本地即可判 401，**不向 SSO 发 introspection 请求**。
///
/// 为什么要预判句柄形状：introspection 默认不缓存（踢人才能下一次请求即 401），连 inactive 结果也不缓存，
/// 若任何非 JWT 形状的 Bearer 都转给它，匿名调用方随便填个字符串就能让资源端对 SSO 发一次 HTTP 往返
/// （最长 10 秒超时），等于把 SSO 暴露成可被放大的目标。形状校验把这类请求挡在本地。
/// 仍然剩下的口子：形状合法的随机句柄会照常被转发——要进一步收敛请开 IntrospectionCacheSeconds 或在网关限流。
/// </summary>
internal static class TokenSchemeSelector
{
    const string BearerPrefix = "Bearer ";

    /// <summary>句柄长度下限/上限。IS4 默认句柄是 43 字符；放宽到区间是为了容忍自定义
    /// <c>IHandleGenerationService</c>（GUID 36、hex 64 等），同时拦住空串/超长垃圾。
    /// 若自定义的句柄生成器输出了区间或字符集之外的形态，Reference 客户端会被本地 401——改生成器时同步改这里</summary>
    internal const int MinReferenceHandleLength = 16;

    internal const int MaxReferenceHandleLength = 128;

    /// <summary>
    /// 按 Authorization 头选 scheme。无 Bearer 前缀 / 无令牌时走 jwt：
    /// 与历史行为一致（jwt 处理器返回 NoResult → 401），且不会为无凭据请求向 SSO 发多余的 introspection 调用。
    /// </summary>
    public static string Resolve(string? authorizationHeader)
    {
        if (string.IsNullOrWhiteSpace(authorizationHeader)
            || !authorizationHeader.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return AuthSchemes.Jwt;
        }

        var token = authorizationHeader[BearerPrefix.Length..].Trim();
        if (LooksLikeJwt(token))
        {
            return AuthSchemes.Jwt;
        }

        // 句柄形状才值得问 SSO；其余（含带 '.' 的畸形 JWT）交给 jwt 处理器本地拒绝
        return LooksLikeReferenceHandle(token) ? AuthSchemes.Introspection : AuthSchemes.Jwt;
    }

    /// <summary>JWT = 三段非空的 '.' 分段（header.payload.signature）</summary>
    internal static bool LooksLikeJwt(string token)
    {
        var parts = token.Split('.');
        return parts.Length == 3 && Array.TrueForAll(parts, part => part.Length > 0);
    }

    /// <summary>reference 句柄 = 长度在 [Min, Max] 内、全部为 base64url 字符集（A-Z a-z 0-9 - _）。
    /// 字符集天然排除了 '.'，所以与 <see cref="LooksLikeJwt"/> 互斥</summary>
    internal static bool LooksLikeReferenceHandle(string token)
    {
        if (token.Length is < MinReferenceHandleLength or > MaxReferenceHandleLength)
        {
            return false;
        }

        foreach (var c in token)
        {
            if (!char.IsAsciiLetterOrDigit(c) && c != '-' && c != '_')
            {
                return false;
            }
        }

        return true;
    }
}

/// <summary>
/// 混合 Bearer 校验注册（Admin 是参考实现，其他 Masa 服务可整体拷贝本文件与接线三行）。
///
/// 背景：踢人（删 auth.PersistedGrant 行）对 JWT 客户端在 AT 过期前不生效（本地验签不问 SSO）；
/// 把高安全客户端配成 AccessTokenType=Reference 后其 AT 是 opaque 句柄，资源端必须走
/// SSO /connect/introspection 才能校验——删行后下一次请求即 401（踢人窗口 = 0 或缓存秒数，见 plan 文档 §7/§8）。
///
/// 三层 scheme：
/// - "jwt"：本地验签（历史行为原样保留，含证书豁免等全部原配置）；
/// - "introspection"：Duende OAuth2Introspection 调 SSO /connect/introspect；
/// - "Bearer"（默认）：PolicyScheme 按 Authorization 头令牌形状转发。
///
/// introspection 的 ClientId/Secret 未配置时只注册 jwt + 静默转发，行为与引入本文件之前**完全一致**——
/// 其他服务接入时可以先合代码、后配密钥，配置生效即切换（仅影响 Reference 客户端）。
/// </summary>
public static class MixedBearerAuthenticationExtensions
{
    /// <summary>DCC/配置键。ClientId/Secret 均配置才启用 introspection（密钥不入仓库，与 Lingua WriteKey 同一约定）</summary>
    public const string IntrospectionClientIdConfigKey = "IdentityServer:IntrospectionClientId";
    public const string IntrospectionClientSecretConfigKey = "IdentityServer:IntrospectionClientSecret";

    /// <summary>introspection 结果缓存秒数，默认 0=关闭（踢人删行后下一次请求即 401）。
    /// 调大可省 SSO 往返，但踢人生效窗口随之变为 ≤ 该秒数（包默认是 5 分钟，对本场景太大）</summary>
    public const string IntrospectionCacheSecondsConfigKey = "IdentityServer:IntrospectionCacheSeconds";

    /// <summary>introspection 后端 HttpClient 超时：与 Webhook 同口径收紧，
    /// 避免 SSO 挂起时拖住资源端热路径（默认 HttpClient 超时 100s 不可接受）</summary>
    static readonly TimeSpan IntrospectionBackchannelTimeout = TimeSpan.FromSeconds(10);

    public static AuthenticationBuilder AddMixedBearerAuthentication(
        this AuthenticationBuilder builder,
        string authority,
        IConfiguration configuration,
        Action<JwtBearerOptions> configureJwt)
    {
        var introspectionClientId = configuration[IntrospectionClientIdConfigKey];
        var introspectionClientSecret = configuration[IntrospectionClientSecretConfigKey];
        var cacheSeconds = configuration.GetValue(IntrospectionCacheSecondsConfigKey, 0);

        builder.AddJwtBearer(AuthSchemes.Jwt, configureJwt);

        if (string.IsNullOrWhiteSpace(introspectionClientId) || string.IsNullOrWhiteSpace(introspectionClientSecret))
        {
            // 未配置密钥：Bearer 静默转发到 jwt，历史行为不变
            builder.AddPolicyScheme(AuthSchemes.Bearer, "JWT", options => options.ForwardDefault = AuthSchemes.Jwt);
            return builder;
        }

        // 包内部已 AddHttpClient(BackChannelHttpClientName)；此处补充证书豁免（镜像 JwtBearer 的 BackchannelHttpHandler）
        // 与超时收紧，追加配置对同名客户端是叠加而非覆盖。
        // .NET 10 移除了 ConfigurePrimaryHttpHandler：主处理器（SocketsHttpHandler）经 UseSocketsHttpHandler 配置
        builder.Services.AddHttpClient(OAuth2IntrospectionDefaults.BackChannelHttpClientName, client =>
            {
                client.Timeout = IntrospectionBackchannelTimeout;
            })
            .UseSocketsHttpHandler(static (handler, _) =>
                // 与 JwtBearer 的 ServerCertificateCustomValidationCallback=>true 等价：豁免自签证书（内网/开发环境）
                handler.SslOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true);

        builder.AddOAuth2Introspection(AuthSchemes.Introspection, options =>
        {
            // discovery 发现 /connect/introspect（IS4 内置端点）。IS4 的 introspection 调用方鉴权约定：
            // ClientId = auth ApiResource 名称、ClientSecret = 该 ApiResource 的 SharedSecret
            options.Authority = authority;
            options.ClientId = introspectionClientId;
            options.ClientSecret = introspectionClientSecret;
            // 镜像 jwt 的 RequireHttpsMetadata=false（内网/自签环境）
            options.DiscoveryPolicy.RequireHttps = false;
            // 与 JwtBearer + MapInboundClaims=false 的取值口径一致：claim 原名透传，
            // MasaIdentity 按 claim 名（IdentityClaimConsts.*）取值，不依赖本设置
            options.NameClaimType = IdentityClaimConsts.USER_NAME;
            options.RoleClaimType = "role";

            // 缓存默认关闭：Duende 7.x 默认缓存 5 分钟（active/inactive 都缓存），会把踢人生效拖到 ≤5 分钟；
            // 禁写 HybridCache 两层（进程内 LocalCache + DistributedCache）即永不命中，每请求实时问 SSO。
            // CacheSeconds>0 时按配置开启：踢人窗口随之变为 ≤CacheSeconds（省 SSO 往返的代价）
            if (cacheSeconds <= 0)
            {
                options.SetCacheEntryFlags = HybridCacheEntryFlags.DisableLocalCacheWrite
                                             | HybridCacheEntryFlags.DisableDistributedCacheWrite;
            }
            else
            {
                options.CacheDuration = TimeSpan.FromSeconds(cacheSeconds);
            }
        });

        // 默认 scheme：按 Authorization 头的令牌形状路由
        builder.AddPolicyScheme(AuthSchemes.Bearer, "JWT or reference token", options =>
        {
            options.ForwardDefaultSelector = context =>
                TokenSchemeSelector.Resolve(context.Request.Headers.Authorization);
        });

        return builder;
    }
}
