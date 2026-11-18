// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary><see cref="DomainAssertionValidator.BuildAudiences"/>：反代场景下 aud 与 Request.Host 可能不一致。</summary>
public class DomainAssertionAudiencesTests
{
    [Fact]
    public void Empty_input_produces_no_audiences()
    {
        Assert.Empty(DomainAssertionValidator.BuildAudiences());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Blank_issuer_is_ignored(string? issuer)
    {
        Assert.Empty(DomainAssertionValidator.BuildAudiences(issuer));
    }

    [Fact]
    public void Issuer_yields_both_endpoint_and_bare_forms()
    {
        var audiences = DomainAssertionValidator.BuildAudiences("https://psso.example.com");

        Assert.Contains("https://psso.example.com", audiences);
        Assert.Contains("https://psso.example.com/connect/token", audiences);
        Assert.Equal(2, audiences.Count);
    }

    [Fact]
    public void Trailing_slash_and_whitespace_are_normalized()
    {
        var audiences = DomainAssertionValidator.BuildAudiences("  https://psso.example.com/  ");

        Assert.Contains("https://psso.example.com/connect/token", audiences);
        Assert.DoesNotContain("https://psso.example.com//connect/token", audiences);
    }

    [Fact]
    public void Multiple_issuers_are_merged_without_duplicates()
    {
        var audiences = DomainAssertionValidator.BuildAudiences(
            "https://psso.example.com",
            "http://auth-sso:8080",
            "https://psso.example.com"); // 重复登记

        Assert.Contains("https://psso.example.com/connect/token", audiences);
        Assert.Contains("http://auth-sso:8080/connect/token", audiences);
        Assert.Equal(4, audiences.Count);
    }

    [Fact]
    public void Blank_entries_do_not_discard_the_valid_ones()
    {
        var audiences = DomainAssertionValidator.BuildAudiences(null, "https://psso.example.com", "  ");

        Assert.Equal(2, audiences.Count);
    }

    [Fact]
    public void Lookup_is_ordinal_case_sensitive_for_hosts()
    {
        // 集合 Ordinal 比较，签发 aud 须与配置形态一致
        var audiences = DomainAssertionValidator.BuildAudiences("https://psso.example.com");

        Assert.Contains("https://psso.example.com/connect/token", audiences);
        Assert.DoesNotContain("HTTPS://PSSO.EXAMPLE.COM/connect/token", audiences);
    }
}
