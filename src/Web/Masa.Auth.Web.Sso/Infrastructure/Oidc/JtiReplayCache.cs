// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Oidc;

/// <summary>RFC 7523 jti 单次性：Redis SET NX，TTL=剩余有效期+余量。Redis 不可用则 fail-closed。</summary>
public class JtiReplayCache
{
    /// <summary>Redis 键前缀：domain-assertion:jti:{clientId}:{jti}</summary>
    public const string KeyPrefix = "domain-assertion:jti:";

    /// <summary>TTL 最小余量：覆盖时钟偏移与断言 exp 之后可能仍在传输中的请求。</summary>
    static readonly TimeSpan MinTtlGrace = TimeSpan.FromSeconds(60);

    readonly IConnectionMultiplexer _mux;
    readonly ILogger<JtiReplayCache> _logger;

    public JtiReplayCache(IConnectionMultiplexer mux, ILogger<JtiReplayCache> logger)
    {
        _mux = mux;
        _logger = logger;
    }

    /// <summary>
    /// 原子登记 jti。返回 <c>true</c> 表示首次使用；<c>false</c> 表示已被用过（重放）。
    /// </summary>
    /// <param name="clockSkew">校验时容忍的时钟偏移：键必须活过 <c>exp + clockSkew</c>，否则偏移窗口内可被重放。</param>
    public virtual async Task<bool> TryRegisterAsync(string clientId, string jti, DateTime expiresUtc, TimeSpan clockSkew = default)
    {
        var key = $"{KeyPrefix}{clientId}:{jti}";
        var grace = clockSkew > MinTtlGrace ? clockSkew : MinTtlGrace;
        var ttl = expiresUtc - DateTime.UtcNow + grace;
        if (ttl <= TimeSpan.Zero)
        {
            // 已过期则不登记，避免零 TTL 键
            return false;
        }

        try
        {
            return await _mux.GetDatabase().StringSetAsync(key, "1", ttl, When.NotExists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Assertion jti registration failed, rejecting the request to stay fail-closed. clientId:{ClientId}",
                clientId);
            throw new UserFriendlyException(errorCode: SsoErrorCodes.DOMAIN_ASSERTION_REPLAY_CHECK_UNAVAILABLE);
        }
    }
}
