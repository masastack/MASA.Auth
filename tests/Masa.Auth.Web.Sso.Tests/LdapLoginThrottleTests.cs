// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Web.Sso.Infrastructure.Ldap;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary><c>grant_type=ldap</c> 限流冻结语义（窗口由 <see cref="ILoginFailureCounter"/> TTL 表达，不顺延）。</summary>
public class LdapLoginThrottleTests
{
    const string Account = "mayue";

    [Fact]
    public async Task Account_is_not_frozen_below_the_threshold()
    {
        var harness = new Harness();

        for (var i = 0; i < LdapLoginThrottle.MaxLoginErrorCount - 1; i++)
        {
            await harness.Throttle.RecordFailureAsync(Account);
        }

        Assert.Null(await Record.ExceptionAsync(() => harness.Throttle.EnsureNotFrozenAsync(Account)));
    }

    [Fact]
    public async Task Account_is_frozen_once_the_threshold_is_reached()
    {
        var harness = new Harness();

        await harness.FailTimesAsync(LdapLoginThrottle.MaxLoginErrorCount);

        var ex = await Assert.ThrowsAsync<UserFriendlyException>(() => harness.Throttle.EnsureNotFrozenAsync(Account));
        Assert.Equal("LoginFreeze", ex.ErrorCode);
    }

    [Fact]
    public async Task Freeze_ends_when_the_window_ends_and_counting_restarts_from_zero()
    {
        var harness = new Harness();
        await harness.FailTimesAsync(LdapLoginThrottle.MaxLoginErrorCount);
        await Assert.ThrowsAsync<UserFriendlyException>(() => harness.Throttle.EnsureNotFrozenAsync(Account));

        harness.Clock.Advance(TimeSpan.FromMinutes(LdapLoginThrottle.LoginFreezeMinutes) + TimeSpan.FromSeconds(1));

        await harness.Throttle.EnsureNotFrozenAsync(Account);
        Assert.Equal(0, await harness.Counter.GetAsync("ldap_login_fail:mayue"));
    }

    [Fact]
    public async Task Failures_after_a_freeze_ends_freeze_the_account_again_after_the_threshold()
    {
        var harness = new Harness();
        await harness.FailTimesAsync(LdapLoginThrottle.MaxLoginErrorCount);
        harness.Clock.Advance(TimeSpan.FromMinutes(LdapLoginThrottle.LoginFreezeMinutes) + TimeSpan.FromSeconds(1));

        for (var i = 0; i < LdapLoginThrottle.MaxLoginErrorCount - 1; i++)
        {
            await harness.Throttle.EnsureNotFrozenAsync(Account);
            await harness.Throttle.RecordFailureAsync(Account);
        }

        await harness.Throttle.EnsureNotFrozenAsync(Account);
        await harness.Throttle.RecordFailureAsync(Account);
        await Assert.ThrowsAsync<UserFriendlyException>(() => harness.Throttle.EnsureNotFrozenAsync(Account));
    }

    [Fact]
    public async Task Later_failures_do_not_extend_the_window()
    {
        var harness = new Harness();
        await harness.Throttle.RecordFailureAsync(Account);

        harness.Clock.Advance(TimeSpan.FromMinutes(LdapLoginThrottle.LoginFreezeMinutes - 1));
        await harness.Throttle.RecordFailureAsync(Account);

        harness.Clock.Advance(TimeSpan.FromMinutes(2));

        Assert.Equal(0, await harness.Counter.GetAsync("ldap_login_fail:mayue"));
    }

    [Fact]
    public async Task Reset_clears_the_failures()
    {
        var harness = new Harness();
        await harness.FailTimesAsync(LdapLoginThrottle.MaxLoginErrorCount);

        await harness.Throttle.ResetAsync(Account);

        Assert.Null(await Record.ExceptionAsync(() => harness.Throttle.EnsureNotFrozenAsync(Account)));
        Assert.Equal(0, await harness.Counter.GetAsync("ldap_login_fail:mayue"));
    }

    [Fact]
    public async Task Account_names_are_case_insensitive()
    {
        var harness = new Harness();

        for (var i = 0; i < LdapLoginThrottle.MaxLoginErrorCount; i++)
        {
            await harness.Throttle.RecordFailureAsync(i % 2 == 0 ? "MaYue" : "mayue");
        }

        await Assert.ThrowsAsync<UserFriendlyException>(() => harness.Throttle.EnsureNotFrozenAsync("MAYUE"));
    }

    [Theory]
    [InlineData("*")]
    [InlineData("a)(b")]
    [InlineData("a\\b")]
    public async Task Invalid_account_names_never_create_counter_keys(string account)
    {
        var harness = new Harness();

        await harness.Throttle.RecordFailureAsync(account);

        Assert.Equal(0, harness.Counter.KeyCount);
    }

    [Fact]
    public async Task Overlong_account_names_never_create_counter_keys()
    {
        var harness = new Harness();

        await harness.Throttle.RecordFailureAsync(new string('a', 10_000));

        Assert.Equal(0, harness.Counter.KeyCount);
    }

    [Fact]
    public async Task Storage_failure_does_not_block_logins_nor_throw_on_record()
    {
        var harness = new Harness { CounterFails = true };

        Assert.Null(await Record.ExceptionAsync(() => harness.Throttle.EnsureNotFrozenAsync(Account)));
        Assert.Null(await Record.ExceptionAsync(() => harness.Throttle.RecordFailureAsync(Account)));
        Assert.Null(await Record.ExceptionAsync(() => harness.Throttle.ResetAsync(Account)));
    }

    sealed class Harness
    {
        readonly FakeCounter _counter;

        public Harness()
        {
            _counter = new FakeCounter(Clock);
            Throttle = new LdapLoginThrottle(_counter, NullLogger<LdapLoginThrottle>.Instance);
        }

        public FakeClock Clock { get; } = new();

        public FakeCounter Counter => _counter;

        public LdapLoginThrottle Throttle { get; }

        public bool CounterFails
        {
            get => _counter.Fails;
            set => _counter.Fails = value;
        }

        public async Task FailTimesAsync(int times)
        {
            for (var i = 0; i < times; i++)
            {
                await Throttle.RecordFailureAsync(Account);
            }
        }
    }

    sealed class FakeClock
    {
        public DateTime Now { get; private set; } = new(2026, 11, 16, 0, 0, 0, DateTimeKind.Utc);

        public void Advance(TimeSpan by) => Now += by;
    }

    /// <summary>内存版 <see cref="ILoginFailureCounter"/>：首增开窗、后续不顺延，过期键消失。</summary>
    sealed class FakeCounter : ILoginFailureCounter
    {
        readonly FakeClock _clock;
        readonly Dictionary<string, (long Count, DateTime ExpiresAt)> _entries = new();

        public FakeCounter(FakeClock clock) => _clock = clock;

        public bool Fails { get; set; }

        public int KeyCount
        {
            get
            {
                Purge();
                return _entries.Count;
            }
        }

        public Task<long> IncrementAsync(string key, TimeSpan window)
        {
            ThrowIfFailing();
            Purge();

            if (_entries.TryGetValue(key, out var entry))
            {
                _entries[key] = (entry.Count + 1, entry.ExpiresAt);
                return Task.FromResult(entry.Count + 1);
            }

            _entries[key] = (1, _clock.Now + window);
            return Task.FromResult(1L);
        }

        public Task<long> GetAsync(string key)
        {
            ThrowIfFailing();
            Purge();
            return Task.FromResult(_entries.TryGetValue(key, out var entry) ? entry.Count : 0L);
        }

        public Task ResetAsync(string key)
        {
            ThrowIfFailing();
            _entries.Remove(key);
            return Task.CompletedTask;
        }

        void Purge()
        {
            foreach (var key in _entries.Where(e => e.Value.ExpiresAt <= _clock.Now).Select(e => e.Key).ToList())
            {
                _entries.Remove(key);
            }
        }

        void ThrowIfFailing()
        {
            if (Fails)
            {
                throw new InvalidOperationException("redis unavailable");
            }
        }
    }
}
