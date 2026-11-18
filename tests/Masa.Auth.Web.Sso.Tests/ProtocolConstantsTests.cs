// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityModel;
using Masa.Auth.Web.Sso.Infrastructure.Ldap;
using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Masa.Auth.Web.Sso.Infrastructure.Validations;
using Xunit;
using ElementPermissionCodes = Masa.Auth.Contracts.Admin.Infrastructure.Constants.ElementPermissionCodes;
using ImpersonationConsts = Masa.Auth.Contracts.Admin.Infrastructure.Constants.ImpersonationConsts;
using SdkGrantType = Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>SSO/Admin/调用方共用的协议字面量锚定，防止一处漂移导致静默失效。</summary>
public class ProtocolConstantsTests
{
    [Fact]
    public void Jwt_bearer_grant_uses_the_RFC7523_urn()
    {
        Assert.Equal("urn:ietf:params:oauth:grant-type:jwt-bearer", SdkGrantType.JWT_BEARER);
        Assert.Equal(OidcConstants.GrantTypes.JwtBearer, SdkGrantType.JWT_BEARER);
    }

    [Fact]
    public void Token_exchange_grant_uses_the_RFC8693_urn()
    {
        Assert.Equal("urn:ietf:params:oauth:grant-type:token-exchange", SdkGrantType.TOKEN_EXCHANGE);
        Assert.Equal(OidcConstants.GrantTypes.TokenExchange, SdkGrantType.TOKEN_EXCHANGE);
    }

    [Fact]
    public void Rfc_grants_are_exposed_by_the_Masa_framework_grant_type_constants()
    {
        // 勾选列表依赖 SDK GrantType 的 public const + Description
        foreach (var name in new[] { "JWT_BEARER", "TOKEN_EXCHANGE" })
        {
            var field = typeof(SdkGrantType).GetField(name);
            Assert.NotNull(field);
            Assert.True(field!.IsLiteral && field.FieldType == typeof(string));
            Assert.NotNull(System.Attribute.GetCustomAttribute(field, typeof(System.ComponentModel.DescriptionAttribute)));
        }
    }

    [Fact]
    public void Custom_subject_token_type_is_namespaced_under_masa()
    {
        // subject_token 标识被模拟用户，不用 access_token 类型
        // 调用方（含文档里的 curl）按字面值传参，常量值不能变
        Assert.Equal("urn:masa:params:oauth:token-type:user-id", ImpersonationConsts.SubjectTokenTypeUserId);
        Assert.StartsWith("urn:masa:params:oauth:token-type:", ImpersonationConsts.SubjectTokenTypeUserId);
        Assert.NotEqual(OidcConstants.TokenTypeIdentifiers.AccessToken, ImpersonationConsts.SubjectTokenTypeUserId);
    }

    [Fact]
    public void Standard_names_come_from_IdentityModel_not_local_copies()
    {
        Assert.Equal("act", JwtClaimTypes.Actor);
        Assert.Equal("amr", JwtClaimTypes.AuthenticationMethod);
        Assert.Equal("urn:ietf:params:oauth:token-type:access_token", OidcConstants.TokenTypeIdentifiers.AccessToken);
        Assert.Equal("wia", OidcConstants.AuthenticationMethods.WindowsIntegratedAuthentication);
    }

    [Fact]
    public void Impersonation_permission_code_is_the_agreed_api_code()
    {
        Assert.Equal("api.user.impersonate", ElementPermissionCodes.ApiUserImpersonate);
        Assert.Equal("user.impersonate", ElementPermissionCodes.UserImpersonate);
    }

    [Fact]
    public void Impersonation_lifetime_is_capped_well_below_the_default_access_token()
    {
        // 模拟登录 access_token TTL 须明显短于默认 3600s
        Assert.InRange(TokenExchangeGrantValidator.MaxAccessTokenLifetimeSeconds, 1, 900);
    }

    [Fact]
    public void Assertion_limits_are_short_enough_to_bound_replay_window()
    {
        Assert.InRange(DomainAssertionValidator.MaxLifetimeSeconds, 1, 300);
        Assert.InRange(DomainAssertionValidator.ClockSkewSeconds, 0, DomainAssertionValidator.MaxLifetimeSeconds - 1);
    }

    [Fact]
    public void Jti_cache_prefix_is_namespaced_to_avoid_collisions()
    {
        Assert.Equal("domain-assertion:jti:", JtiReplayCache.KeyPrefix);
    }

    [Fact]
    public void Domain_assertion_keys_use_the_JWK_secret_type_the_admin_UI_exposes()
    {
        // Admin 与 IS4 的 JWK secret 类型字面值须一致
        Assert.Equal("JWK", IdentityServer4.IdentityServerConstants.SecretTypes.JsonWebKey);
        Assert.Contains("JWK", Masa.Auth.Contracts.Admin.Infrastructure.Constants.ClientConsts
            .GetSecretTypes().Select(s => s.Value));
    }

    [Fact]
    public void Client_property_keys_are_explicit_and_namespaced()
    {
        Assert.Equal("domain_assertion.account_pattern", DomainAccountMatching.AccountPatternProperty);
    }
}
