// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Web.Sso.Infrastructure.Ldap;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>账号名进入 LDAP 过滤器 / Redis 键之前的校验与归一化。</summary>
public class DomainAccountNameTests
{
    [Theory]
    [InlineData("mayue")]
    [InlineData("zhang.san")]
    [InlineData("svc_report-01")]
    [InlineData("张三")]
    public void Ordinary_account_names_are_valid(string account)
    {
        Assert.True(DomainAccountName.IsValid(account));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Blank_account_names_are_invalid(string? account)
    {
        Assert.False(DomainAccountName.IsValid(account));
    }

    [Theory]
    [InlineData("*")]                       // 通配：匹配任意条目
    [InlineData("a*")]
    [InlineData("admin)(objectClass=*")]    // 过滤器注入
    [InlineData("DOMAIN\\user")]
    [InlineData("user@domain.com")]
    [InlineData("a/b")]
    [InlineData("a,b")]
    [InlineData("a=b")]
    [InlineData("a|b")]                     // 同时是缓存值分隔符
    [InlineData("a:b")]
    [InlineData("a\0b")]
    [InlineData("a\nb")]
    public void Ldap_metacharacters_and_control_characters_are_rejected(string account)
    {
        Assert.False(DomainAccountName.IsValid(account));
    }

    [Fact]
    public void Length_limit_is_256()
    {
        Assert.True(DomainAccountName.IsValid(new string('a', 256)));
        Assert.False(DomainAccountName.IsValid(new string('a', 257)));
    }

    [Theory]
    [InlineData("mayue", "mayue")]
    [InlineData("  mayue  ", "mayue")]
    [InlineData("LONSID\\mayue", "mayue")]
    [InlineData("mayue@lonsid.cn", "mayue")]
    [InlineData("LONSID\\mayue@lonsid.cn", "mayue")]
    [InlineData(null, "")]
    [InlineData("   ", "")]
    public void Domain_prefix_and_suffix_are_stripped(string? raw, string expected)
    {
        Assert.Equal(expected, DomainAccountName.Normalize(raw));
    }

    [Theory]
    [InlineData("LONSID\\")]    // 只有前缀：保留原样，随后在 IsValid 处被拒绝
    [InlineData("@lonsid.cn")]  // 只有后缀：同上
    public void Degenerate_input_is_not_turned_into_a_valid_name(string raw)
    {
        Assert.False(DomainAccountName.IsValid(DomainAccountName.Normalize(raw)));
    }
}
