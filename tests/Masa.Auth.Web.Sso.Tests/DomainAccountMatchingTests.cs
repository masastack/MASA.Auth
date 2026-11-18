// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Web.Sso.Infrastructure.Ldap;
using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>account_pattern 全串锚定匹配语义。</summary>
public class DomainAccountMatchingTests
{
    [Theory]
    [InlineData(null, "mayue", true)]
    [InlineData("", "mayue", true)]
    [InlineData("   ", "mayue", true)]
    public void Empty_pattern_means_unrestricted(string? pattern, string account, bool expected)
    {
        Assert.Equal(expected, DomainAccountMatching.IsAllowed(pattern, account));
    }

    [Theory]
    [InlineData("mayue", "mayue", true)]
    [InlineData("MAYUE", "mayue", true)]
    [InlineData("mayue", "MAYUE", true)]
    [InlineData("mayue", "mayue2", false)]
    public void Exact_pattern_is_case_insensitive_and_anchored(string pattern, string account, bool expected)
    {
        Assert.Equal(expected, DomainAccountMatching.IsAllowed(pattern, account));
    }

    [Theory]
    [InlineData("mayue|zhangsan", "zhangsan", true)]
    [InlineData("mayue|zhangsan", "lisi", false)]
    [InlineData(" mayue | zhangsan ", "zhangsan", true)]
    [InlineData("mayue||zhangsan", "lisi", false)]
    public void Multiple_patterns_are_pipe_separated(string pattern, string account, bool expected)
    {
        Assert.Equal(expected, DomainAccountMatching.IsAllowed(pattern, account));
    }

    [Theory]
    [InlineData("zhang*", "zhangsan.admin", true)]
    [InlineData("zhang*", "mayue", false)]
    [InlineData("*.svc", "backup.svc", true)]
    [InlineData("*.svc", "backup.svc2", false)]
    [InlineData("user?", "user1", true)]
    [InlineData("user?", "user12", false)]
    public void Wildcards_are_supported_but_still_anchored(string pattern, string account, bool expected)
    {
        Assert.Equal(expected, DomainAccountMatching.IsAllowed(pattern, account));
    }

    [Fact]
    public void Substring_is_not_enough_to_match()
    {
        // 防止退化为未锚定子串匹配
        Assert.False(DomainAccountMatching.IsAllowed("zhang", "zhangsan.admin"));
        Assert.False(DomainAccountMatching.IsAllowed("san", "zhangsan"));
    }
}
