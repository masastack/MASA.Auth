// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Masa.Auth.Web.Sso.Infrastructure.Ldap;
using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Masa.Auth.Web.Sso.Infrastructure.Validations;
using Masa.BuildingBlocks.StackSdks.Auth;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
using Masa.BuildingBlocks.StackSdks.Auth.Service;
using Masa.BuildingBlocks.StackSdks.Config.Consts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Xunit;
using SdkGrantType = Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>JWT Bearer 断言 grant 编排：grant/jti/sub 范围/AD/MASA/audience；断言 IsError 与下游未调用。</summary>
public class JwtBearerAssertionGrantValidatorTests
{
    const string ClientId = "domainidentity";
    const string PublicUrl = "https://sso.example.com";
    static readonly Guid MasaUserId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    [Fact]
    public async Task Valid_assertion_issues_a_token_with_a_single_wia_amr()
    {
        var harness = new Harness();
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
        Assert.Equal(MasaUserId.ToString(), context.Result.Subject!.FindFirst(JwtClaimTypes.Subject)!.Value);
        Assert.Equal("mayue", context.Result.Subject.FindFirst(IdentityClaimConsts.DOMAIN_NAME)!.Value);

        var amr = Assert.Single(context.Result.Subject.FindAll(JwtClaimTypes.AuthenticationMethod));
        Assert.Equal(OidcConstants.AuthenticationMethods.WindowsIntegratedAuthentication, amr.Value);

        var registration = Assert.Single(harness.Jti.Registrations);
        Assert.Equal(ClientId, registration.ClientId);
        Assert.Equal(harness.LastJti, registration.Jti);
        Assert.Equal(TimeSpan.FromSeconds(DomainAssertionValidator.ClockSkewSeconds), registration.ClockSkew);
        Assert.Equal(1, harness.AuthClient.OperationLogs);
    }

    [Fact]
    public async Task Netbios_prefixed_subject_is_normalized_before_the_AD_lookup()
    {
        var harness = new Harness();
        var context = harness.CreateContext(harness.CreateAssertion(sub: "LONSID\\mayue"));

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
        Assert.Equal("mayue", Assert.Single(harness.Resolver.LookedUp));
    }

    [Fact]
    public async Task Public_client_is_rejected_even_with_a_perfectly_valid_assertion()
    {
        var harness = new Harness { RequireClientSecret = false };
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.Jti.Registrations);
        Assert.Empty(harness.Resolver.LookedUp);
    }

    [Fact]
    public async Task Assertion_for_another_environment_is_rejected_and_does_not_burn_a_jti()
    {
        var harness = new Harness();
        var context = harness.CreateContext(harness.CreateAssertion(audience: "https://other-sso.example.com/connect/token"));

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.Jti.Registrations);
        Assert.Empty(harness.Resolver.LookedUp);
    }

    [Fact]
    public async Task Replayed_assertion_is_rejected_before_the_AD_lookup()
    {
        var harness = new Harness { JtiIsFirstUse = false };
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Single(harness.Jti.Registrations);
        Assert.Empty(harness.Resolver.LookedUp);
    }

    [Theory]
    [InlineData("*")]
    [InlineData("a*")]
    [InlineData("mayue)(objectClass=*")]
    [InlineData("may|ue")]
    public async Task Subject_with_ldap_metacharacters_never_reaches_the_directory(string sub)
    {
        var harness = new Harness();
        var context = harness.CreateContext(harness.CreateAssertion(sub: sub));

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.Resolver.LookedUp);
        Assert.Empty(harness.Jti.Registrations);
    }

    [Fact]
    public async Task Account_outside_the_clients_pattern_is_rejected_before_the_AD_lookup()
    {
        var harness = new Harness { AccountPattern = "svc-*" };
        var context = harness.CreateContext(harness.CreateAssertion(sub: "mayue"));

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.Resolver.LookedUp);
    }

    [Fact]
    public async Task Account_inside_the_clients_pattern_is_allowed()
    {
        var harness = new Harness { AccountPattern = "may*|svc-*" };
        var context = harness.CreateContext(harness.CreateAssertion(sub: "mayue"));

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
    }

    [Fact]
    public async Task Account_missing_in_AD_is_rejected()
    {
        var harness = new Harness { ResolvedAccount = null };
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Equal(0, harness.AuthClient.ThirdPartyLookups);
    }

    [Fact]
    public async Task Account_disabled_in_AD_is_rejected()
    {
        var harness = new Harness { ResolvedAccount = new DomainAccountResolver.ResolvedAccount("mayue", "ad-guid-1", IsDisabled: true) };
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Equal(0, harness.AuthClient.ThirdPartyLookups);
    }

    [Fact]
    public async Task Account_not_provisioned_in_MASA_is_rejected_without_creating_a_user()
    {
        var harness = new Harness { MasaUserExists = false };
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Equal(0, harness.AuthClient.UserCreations);
    }

    [Fact]
    public async Task User_disabled_in_MASA_is_rejected()
    {
        var harness = new Harness { MasaEnabled = false };
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Null(context.Result.Subject);
        Assert.Equal(0, harness.AuthClient.OperationLogs);
    }

    [Fact]
    public async Task User_deleted_in_MASA_is_rejected()
    {
        var harness = new Harness { MasaDeleted = true };
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Null(context.Result.Subject);
    }

    [Fact]
    public async Task Infrastructure_failure_does_not_leak_internal_details()
    {
        var harness = new Harness { ResolverException = new InvalidOperationException("connect to ldap-internal.corp:389 failed") };
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.DoesNotContain("ldap-internal", context.Result.ErrorDescription);
    }

    [Fact]
    public async Task Stack_sso_domain_is_authoritative_and_a_spoofed_host_header_is_not_accepted()
    {
        var harness = new Harness { RequestHost = "evil.example.com" };
        var context = harness.CreateContext(harness.CreateAssertion(audience: "https://evil.example.com/connect/token"));

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Request_host_is_only_a_fallback_when_the_stack_has_no_sso_domain()
    {
        var harness = new Harness { StackSsoDomain = null, RequestHost = "sso.local" };
        var context = harness.CreateContext(harness.CreateAssertion(audience: "https://sso.local/connect/token"));

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
    }

    [Fact]
    public async Task Rotated_out_signing_key_no_longer_validates_assertions()
    {
        var harness = new Harness { KeyExpiredAt = DateTime.UtcNow.AddMinutes(-1) };
        var context = harness.CreateContext(harness.CreateAssertion());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.Jti.Registrations);
    }

    sealed class Harness
    {
        readonly RSA _rsa = RSA.Create(2048);

        public bool RequireClientSecret { get; set; } = true;
        public string? AccountPattern { get; set; }
        public bool JtiIsFirstUse { get; set; } = true;
        public bool MasaUserExists { get; set; } = true;
        public bool MasaEnabled { get; set; } = true;
        public bool MasaDeleted { get; set; }
        public string? StackSsoDomain { get; set; } = PublicUrl;
        public string RequestHost { get; set; } = "sso.internal:8080";
        public DateTime? KeyExpiredAt { get; set; }
        public Exception? ResolverException { get; set; }

        public DomainAccountResolver.ResolvedAccount? ResolvedAccount { get; set; }
            = new("mayue", "ad-guid-1", IsDisabled: false);

        public string? LastJti { get; private set; }

        public StubJtiReplayCache Jti { get; }
        public StubResolver Resolver { get; }
        public AuthClientProxy AuthClient { get; }

        readonly IAuthClient _authClientProxy;
        JwtBearerAssertionGrantValidator? _validator;

        /// <summary>延迟构造 Validator：测试在初始化器里才设置 StackSsoDomain。</summary>
        public JwtBearerAssertionGrantValidator Validator => _validator ??= new JwtBearerAssertionGrantValidator(
            _authClientProxy,
            Resolver,
            Jti,
            new StubHttpContextAccessor(() => RequestHost),
            StubMasaStackConfig.WithSsoDomain(StackSsoDomain),
            NullLogger<JwtBearerAssertionGrantValidator>.Instance);

        public Harness()
        {
            Jti = new StubJtiReplayCache(this);
            Resolver = new StubResolver(this);
            _authClientProxy = DispatchProxy.Create<IAuthClient, AuthClientProxy>();
            AuthClient = (AuthClientProxy)(object)_authClientProxy;
            AuthClient.Owner = this;
        }

        public string CreateAssertion(string sub = "mayue", string audience = $"{PublicUrl}/connect/token")
        {
            LastJti = Guid.NewGuid().ToString("N");
            var now = DateTime.UtcNow;
            return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
            {
                Issuer = ClientId,
                Audience = audience,
                Subject = new ClaimsIdentity(new[] { new Claim("sub", sub) }),
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddSeconds(60),
                Claims = new Dictionary<string, object> { ["jti"] = LastJti },
                SigningCredentials = new SigningCredentials(
                    new RsaSecurityKey(_rsa) { KeyId = "k1" },
                    SecurityAlgorithms.RsaSha256)
            });
        }

        public ExtensionGrantValidationContext CreateContext(string assertion)
        {
            var publicJwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(
                new RsaSecurityKey(_rsa.ExportParameters(false)) { KeyId = "k1" });

            var client = new Client
            {
                ClientId = ClientId,
                RequireClientSecret = RequireClientSecret,
                AllowedGrantTypes = new[] { SdkGrantType.JWT_BEARER },
                ClientSecrets =
                {
                    new Secret
                    {
                        Value = System.Text.Json.JsonSerializer.Serialize(publicJwk),
                        Type = IdentityServer4.IdentityServerConstants.SecretTypes.JsonWebKey,
                        Expiration = KeyExpiredAt
                    }
                }
            };

            if (AccountPattern is not null)
            {
                client.Properties[DomainAccountMatching.AccountPatternProperty] = AccountPattern;
            }

            return new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Client = client,
                    GrantType = SdkGrantType.JWT_BEARER,
                    Raw = new System.Collections.Specialized.NameValueCollection { ["assertion"] = assertion }
                }
            };
        }

        public UserModel? BuildMasaUser() => MasaUserExists
            ? new UserModel { Id = MasaUserId, Account = "mayue", DisplayName = "Ma Yue", Enabled = MasaEnabled, IsDeleted = MasaDeleted }
            : null;
    }

    sealed record Registration(string ClientId, string Jti, TimeSpan ClockSkew);

    sealed class StubJtiReplayCache : JtiReplayCache
    {
        readonly Harness _owner;

        public StubJtiReplayCache(Harness owner) : base(null!, NullLogger<JtiReplayCache>.Instance) => _owner = owner;

        public List<Registration> Registrations { get; } = new();

        public override Task<bool> TryRegisterAsync(string clientId, string jti, DateTime expiresUtc, TimeSpan clockSkew = default)
        {
            Registrations.Add(new Registration(clientId, jti, clockSkew));
            return Task.FromResult(_owner.JtiIsFirstUse);
        }
    }

    sealed class StubResolver : DomainAccountResolver
    {
        readonly Harness _owner;

        public StubResolver(Harness owner) : base(null!, null!, null!, NullLogger<DomainAccountResolver>.Instance) => _owner = owner;

        public List<string> LookedUp { get; } = new();

        public override Task<ResolvedAccount?> ResolveAsync(string account)
        {
            LookedUp.Add(account);
            if (_owner.ResolverException is not null)
            {
                throw _owner.ResolverException;
            }

            return Task.FromResult(_owner.ResolvedAccount);
        }
    }

    sealed class StubHttpContextAccessor : IHttpContextAccessor
    {
        readonly Func<string> _host;

        public StubHttpContextAccessor(Func<string> host) => _host = host;

        public HttpContext? HttpContext
        {
            get => BuildContext();
            set { }
        }

        HttpContext BuildContext()
        {
            var context = new DefaultHttpContext();
            context.Request.Scheme = "https";
            context.Request.Host = new HostString(_host());
            return context;
        }
    }

    class AuthClientProxy : DispatchProxy
    {
        public Harness Owner { get; set; } = null!;
        public int ThirdPartyLookups { get; set; }
        public int UserCreations { get; set; }
        public int OperationLogs { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            switch (targetMethod?.Name)
            {
                case "get_UserService":
                    var userService = DispatchProxy.Create<IUserService, UserServiceProxy>();
                    ((UserServiceProxy)(object)userService).Client = this;
                    return userService;

                case "get_OperationLogService":
                    var logService = DispatchProxy.Create<IOperationLogService, OperationLogServiceProxy>();
                    ((OperationLogServiceProxy)(object)logService).Client = this;
                    return logService;
            }

            throw new Xunit.Sdk.XunitException($"Unexpected call to IAuthClient.{targetMethod?.Name}");
        }
    }

    class UserServiceProxy : DispatchProxy
    {
        public AuthClientProxy Client { get; set; } = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            switch (targetMethod?.Name)
            {
                case nameof(IUserService.GetThirdPartyUserAsync):
                    Client.ThirdPartyLookups++;
                    return Task.FromResult(Client.Owner.BuildMasaUser());

                case nameof(IUserService.AddThirdPartyUserAsync):
                    Client.UserCreations++;
                    throw new Xunit.Sdk.XunitException("The assertion channel must not create users (no JIT provisioning).");
            }

            throw new Xunit.Sdk.XunitException($"Unexpected call to IUserService.{targetMethod?.Name}");
        }
    }

    class OperationLogServiceProxy : DispatchProxy
    {
        public AuthClientProxy Client { get; set; } = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            Client.OperationLogs++;
            return Task.CompletedTask;
        }
    }
}
