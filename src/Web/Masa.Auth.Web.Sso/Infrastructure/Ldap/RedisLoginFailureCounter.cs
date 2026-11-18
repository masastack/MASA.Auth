// Copyright (c) MASA Stack Authors. Licensed under the Apache License, Version 2.0.

namespace Masa.Auth.Web.Sso.Infrastructure.Ldap;

/// <summary>
/// <see cref="ILoginFailureCounter"/> 的 Redis 实现；Lua 仅在 <c>PTTL&lt;0</c> 时设窗，后续 INCR 不顺延 TTL。
/// </summary>
public class RedisLoginFailureCounter : ILoginFailureCounter
{
    // KEYS[1] = 计数键；ARGV[1] = 窗口毫秒数
    const string IncrementScript = @"
local count = redis.call('INCR', KEYS[1])
if redis.call('PTTL', KEYS[1]) < 0 then
    redis.call('PEXPIRE', KEYS[1], ARGV[1])
end
return count";

    readonly IConnectionMultiplexer _mux;

    public RedisLoginFailureCounter(IConnectionMultiplexer mux)
    {
        _mux = mux;
    }

    public async Task<long> IncrementAsync(string key, TimeSpan window)
    {
        var result = await _mux.GetDatabase().ScriptEvaluateAsync(
            IncrementScript,
            new RedisKey[] { key },
            new RedisValue[] { (long)window.TotalMilliseconds });

        return (long)result;
    }

    public async Task<long> GetAsync(string key)
    {
        var value = await _mux.GetDatabase().StringGetAsync(key);
        return value.HasValue && long.TryParse(value.ToString(), out var count) ? count : 0;
    }

    public Task ResetAsync(string key) => _mux.GetDatabase().KeyDeleteAsync(key);
}
