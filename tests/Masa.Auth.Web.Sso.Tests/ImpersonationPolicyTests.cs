// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License, Version 2.0.

using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Masa.Auth.Web.Sso.Infrastructure.Validations;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary><see cref="ImpersonationPolicy"/> 单元测试（offline_access、amr、act）。TTL 上限由 <c>TokenExchangeGrantValidatorTests</c> 与 <c>Is4PipelineTests</c> 覆盖。</summary>
public class ImpersonationPolicyTests
{
    [Theory]
    [InlineData("openid profile offline_access", true)]
    [InlineData("offline_access", true)]
    [InlineData("openid profile", false)]
    [InlineData(null, false)]
    [InlineData("", false)]
    public void Offline_access_in_raw_scope_is_detected(string? rawScope, bool expected)
    {
        Assert.Equal(expected, ImpersonationPolicy.RequestsOfflineAccess(null, rawScope));
    }

    [Fact]
    public void Offline_access_in_parsed_scopes_is_detected_even_without_raw_scope()
    {
        Assert.True(ImpersonationPolicy.RequestsOfflineAccess(new[] { "openid", "offline_access" }, null));
        Assert.False(ImpersonationPolicy.RequestsOfflineAccess(new[] { "openid", "profile" }, "openid"));
    }

    [Fact]
    public void Denied_actor_methods_cover_passwordless_assertions()
    {
        var denied = TokenExchangeGrantValidator.DeniedActorAuthenticationMethods;

        Assert.Contains("wia", denied);
    }

    [Fact]
    public void Actor_authentication_methods_on_the_deny_list_are_reported_case_insensitively()
    {
        var found = ImpersonationPolicy.FindDeniedAuthenticationMethods(
            new[] { "pwd", "WIA", "wia" },
            new[] { "wia", "impersonation" });

        Assert.Equal(new[] { "WIA" }, found);
    }

    [Fact]
    public void Empty_deny_list_allows_every_authentication_method()
    {
        Assert.Empty(ImpersonationPolicy.FindDeniedAuthenticationMethods(
            new[] { "wia", "impersonation" },
            Array.Empty<string>()));
    }

    [Fact]
    public void Act_claim_carries_the_actor_subject_as_json_object()
    {
        var actorId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        var act = ImpersonationPolicy.BuildActClaim(actorId);

        using var doc = System.Text.Json.JsonDocument.Parse(act);
        Assert.Equal(actorId.ToString(), doc.RootElement.GetProperty("sub").GetString());
    }
}
