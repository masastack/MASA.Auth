// Copyright (c) MASA Stack Authors. Licensed under the Apache License, Version 2.0.

using Masa.Auth.Contracts.Admin.Sso;
using Xunit;
using SdkGrantType = Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>管理界面 grant 勾选列表须包含 RFC 7523/8693 等规范 grant。</summary>
public class ClientAuthenticationGrantTypeTests
{
    /// <summary>已下线的自定义 grant impersonation。</summary>
    const string RetiredImpersonationGrant = "impersonation";

    static List<string> GrantTypeIds() => new ClientAuthenticationDto().GrantTypes.Select(g => g.Id).ToList();

    [Fact]
    public void Jwt_bearer_assertion_grant_is_selectable()
    {
        Assert.Contains(SdkGrantType.JWT_BEARER, GrantTypeIds());
    }

    [Fact]
    public void Token_exchange_grant_is_selectable()
    {
        Assert.Contains(SdkGrantType.TOKEN_EXCHANGE, GrantTypeIds());
    }

    [Fact]
    public void Retired_impersonation_grant_is_no_longer_selectable()
    {
        Assert.DoesNotContain(RetiredImpersonationGrant, GrantTypeIds());
    }

    [Fact]
    public void Saving_a_client_that_still_lists_the_retired_grant_drops_it_and_keeps_the_rest()
    {
        var dto = new ClientAuthenticationDto();

        dto.AllowedGrantTypes = new List<string>
        {
            RetiredImpersonationGrant,
            "ldap",
            SdkGrantType.TOKEN_EXCHANGE
        };

        var saved = dto.AllowedGrantTypes;
        Assert.DoesNotContain(RetiredImpersonationGrant, saved);
        Assert.Contains("ldap", saved);
        Assert.Contains(SdkGrantType.TOKEN_EXCHANGE, saved);
    }

    [Fact]
    public void Existing_grant_types_are_still_offered()
    {
        // 回归：新增 grant 不得误删既有项
        var ids = GrantTypeIds();
        Assert.Contains("authorization_code", ids);
        Assert.Contains("client_credentials", ids);
        Assert.Contains("password", ids);
        Assert.Contains("ldap", ids);
        Assert.Contains("phone_code", ids);
        Assert.Contains("third_party_idp", ids);
        Assert.Contains("email_code", ids);
    }

    [Fact]
    public void Grant_type_ids_are_unique()
    {
        var ids = GrantTypeIds();
        Assert.Equal(ids.Count, ids.Distinct(StringComparer.Ordinal).Count());
    }

    [Theory]
    [InlineData("JWK", true)]
    [InlineData("jwk", true)]
    [InlineData("X509CertificateBase64", true)]
    [InlineData("X509Name", true)]
    [InlineData("X509Thumbprint", true)]
    [InlineData("SharedSecret", false)]
    [InlineData(null, false)]
    public void Only_shared_secrets_are_hashed_on_save(string? secretType, bool mustBePreserved)
    {
        // 公钥材料哈希后无法验签
        Assert.Equal(mustBePreserved, Masa.Auth.Contracts.Admin.Infrastructure.Constants.ClientConsts.IsVerificationKeySecret(secretType));
    }

    [Fact]
    public void Every_offered_asymmetric_secret_type_is_preserved_on_save()
    {
        // 除 SharedSecret 外均为须原样保存的公钥类型
        var types = Masa.Auth.Contracts.Admin.Infrastructure.Constants.ClientConsts.GetSecretTypes().Select(s => s.Value);
        foreach (var type in types)
        {
            Assert.Equal(
                type != "SharedSecret",
                Masa.Auth.Contracts.Admin.Infrastructure.Constants.ClientConsts.IsVerificationKeySecret(type));
        }
    }
}
