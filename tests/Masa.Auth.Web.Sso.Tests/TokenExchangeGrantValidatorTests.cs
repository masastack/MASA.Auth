// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Reflection;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Masa.Auth.Web.Sso.Infrastructure.Validations;
using Masa.BuildingBlocks.StackSdks.Auth;
using Masa.BuildingBlocks.StackSdks.Auth.Service;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using SdkGrantType = Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>
/// <see cref="TokenExchangeGrantValidator"/> 集成式测试；策略细节见 <see cref="ImpersonationPolicyTests"/>。
/// 拒绝场景断言 <c>IsError</c>（对外统一 invalid_grant）。
/// </summary>
public class TokenExchangeGrantValidatorTests
{
    static readonly Guid ActorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    static readonly Guid TargetId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task Missing_actor_token_is_rejected()
    {
        var harness = new Harness();
        var context = harness.CreateContext(actorToken: null);

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Missing_actor_token_type_is_rejected()
    {
        var harness = new Harness();
        var context = harness.CreateContext(actorTokenType: null);

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Wrong_actor_token_type_is_rejected()
    {
        var harness = new Harness();
        var context = harness.CreateContext(actorTokenType: "urn:ietf:params:oauth:token-type:jwt");

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Unsupported_requested_token_type_is_rejected()
    {
        var harness = new Harness();
        var context = harness.CreateContext(requestedTokenType: "urn:ietf:params:oauth:token-type:refresh_token");

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Invalid_actor_token_is_rejected()
    {
        var harness = new Harness { ActorTokenIsValid = false };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Public_client_is_rejected()
    {
        var harness = new Harness { RequireClientSecret = false };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Actor_without_the_impersonate_permission_is_rejected()
    {
        var harness = new Harness { ActorAuthorized = false };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Equal(ActorId, Assert.Single(harness.PermissionCheckUserIds));
    }

    [Fact]
    public async Task Chained_impersonation_is_rejected()
    {
        var harness = new Harness { ActorTokenHasActClaim = true };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Requesting_offline_access_is_rejected()
    {
        var harness = new Harness();
        var context = harness.CreateContext(scope: "openid profile offline_access");

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Missing_subject_token_is_rejected()
    {
        var harness = new Harness();
        var context = harness.CreateContext(omitSubjectToken: true);

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Missing_subject_token_type_is_rejected()
    {
        var harness = new Harness();
        var context = harness.CreateContext(subjectTokenType: null);

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Wrong_subject_token_type_is_rejected()
    {
        var harness = new Harness();
        var context = harness.CreateContext(subjectTokenType: "urn:ietf:params:oauth:token-type:access_token");

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Impersonating_yourself_is_rejected()
    {
        var harness = new Harness();
        var context = harness.CreateContext(subjectToken: ActorId.ToString());

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    [Fact]
    public async Task Disabled_target_user_is_rejected()
    {
        var harness = new Harness { TargetEnabled = false };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
    }

    /// <summary>
    /// 固化信任模型：只校验 actor 持有 <c>api.user.impersonate</c>，不查询、不比较目标与 actor 的角色/权限高低。
    /// 目标可以拥有 actor 没有的权限（例如内置 admin），模拟照常放行；若以后有人加了防提权查询，
    /// UserServiceProxy 会对未登记的调用直接抛错，这里会失败。
    /// </summary>
    [Fact]
    public async Task Target_privileges_are_not_compared_with_the_actor()
    {
        var harness = new Harness();
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
        Assert.All(harness.UserServiceCalls, call => Assert.Equal(nameof(IUserService.GetDetailAsync), call));
    }

    [Fact]
    public async Task Permission_check_receives_the_actor_user_id()
    {
        var harness = new Harness
        {
            AccessTokenLifetimeSeconds = 3600
        };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
        Assert.Equal(ActorId, Assert.Single(harness.PermissionCheckUserIds));
    }

    [Fact]
    public async Task Success_issues_act_claim_amr_and_caps_the_lifetime()
    {
        var harness = new Harness
        {
            AccessTokenLifetimeSeconds = 7200
        };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);

        Assert.Equal(TargetId.ToString(), context.Result.Subject!.FindFirst(JwtClaimTypes.Subject)!.Value);

        var amr = Assert.Single(context.Result.Subject.FindAll(JwtClaimTypes.AuthenticationMethod));
        Assert.Equal("impersonation", amr.Value);

        var act = Assert.Single(context.Result.Subject.FindAll(JwtClaimTypes.Actor));
        Assert.Contains(ActorId.ToString(), act.Value);

        // act 须为 JSON 对象 claim（valueType=Json）
        Assert.Equal(IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json, act.ValueType);
        using var actJson = System.Text.Json.JsonDocument.Parse(act.Value);
        Assert.Equal(System.Text.Json.JsonValueKind.Object, actJson.RootElement.ValueKind);
        Assert.Equal(ActorId.ToString(), actJson.RootElement.GetProperty("sub").GetString());

        Assert.Equal(TokenExchangeGrantValidator.MaxAccessTokenLifetimeSeconds, context.Request.AccessTokenLifetime);

        Assert.Equal(
            OidcConstants.TokenTypeIdentifiers.AccessToken,
            context.Result.CustomResponse![OidcConstants.TokenResponse.IssuedTokenType]?.ToString());
    }

    [Fact]
    public async Task Shorter_client_lifetime_is_kept_not_extended_to_the_cap()
    {
        var harness = new Harness
        {
            AccessTokenLifetimeSeconds = 120
        };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
        Assert.Equal(120, context.Request.AccessTokenLifetime);
    }

    [Fact]
    public async Task Infrastructure_failures_do_not_leak_their_message_to_the_caller()
    {
        var harness = new Harness { ThrowOnGetDetail = new InvalidOperationException("http://10.0.0.7:8080 connection refused") };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.DoesNotContain("10.0.0.7", context.Result.ErrorDescription);
    }

    [Fact]
    public async Task Disabled_actor_with_an_unexpired_token_is_rejected()
    {
        var harness = new Harness { ActorEnabled = false };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.PermissionCheckUserIds);
    }

    [Fact]
    public async Task Deleted_actor_is_rejected()
    {
        var harness = new Harness { ActorDeleted = true };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.PermissionCheckUserIds);
    }

    [Fact]
    public async Task Unknown_actor_is_rejected()
    {
        var harness = new Harness { ActorExists = false };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.PermissionCheckUserIds);
    }

    [Theory]
    [InlineData("wia")]
    [InlineData("WIA")]
    public async Task Actor_token_from_a_passwordless_login_is_rejected(string amr)
    {
        var harness = new Harness { ActorAuthenticationMethods = new[] { "pwd", amr } };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.PermissionCheckUserIds);
    }

    [Fact]
    public async Task Actor_token_is_forwarded_to_the_auth_service_for_the_actor_and_the_target_lookups()
    {
        // /connect/token 上没有 Bearer，api/user/byId 需要登录身份：两次查询都须以 actor_token 转发
        var harness = new Harness();
        var context = harness.CreateContext(actorToken: "the-actor-token");

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
        Assert.Equal(new[] { "the-actor-token", "the-actor-token" }, harness.TokensSeenByGetDetail);
    }

    [Fact]
    public async Task Actor_token_is_not_forwarded_when_it_failed_validation()
    {
        // 验签失败的 token 不得发给 Auth 服务
        var harness = new Harness { ActorTokenIsValid = false };
        var context = harness.CreateContext(actorToken: "forged-token");

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Empty(harness.TokensSeenByGetDetail);
    }

    [Fact]
    public async Task Actor_token_from_another_client_is_accepted()
    {
        var harness = new Harness { ActorTokenClientId = "some-other-client" };
        var context = harness.CreateContext();

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
    }

    sealed class Harness
    {
        public bool ActorTokenIsValid { get; set; } = true;
        public bool ActorTokenHasActClaim { get; set; }
        public string[] ActorAuthenticationMethods { get; set; } = new[] { "pwd" };
        public string ActorTokenClientId { get; set; } = "masa-admin";
        public bool ActorExists { get; set; } = true;
        public bool ActorEnabled { get; set; } = true;
        public bool ActorDeleted { get; set; }
        public bool RequireClientSecret { get; set; } = true;
        public Exception? ThrowOnGetDetail { get; set; }
        public bool ActorAuthorized { get; set; } = true;
        public bool TargetEnabled { get; set; } = true;
        public List<string> UserServiceCalls { get; } = new();
        public int AccessTokenLifetimeSeconds { get; set; } = 3600;

        /// <summary>SDK 转发给 Auth 服务的 Bearer 来源，由 <see cref="ActorPermissionClient"/> 在查询用户时改写。</summary>
        public Masa.Contrib.StackSdks.Caller.TokenProvider TokenProvider { get; } = new();

        /// <summary>每次 <c>GetDetailAsync</c> 调用瞬间 <see cref="TokenProvider"/> 上的 token。</summary>
        public List<string?> TokensSeenByGetDetail { get; } = new();

        public StubActorPermissionClient PermissionClient { get; }

        public IReadOnlyList<Guid> PermissionCheckUserIds => PermissionClient.RequestedUserIds;

        TokenExchangeGrantValidator? _validator;

        public TokenExchangeGrantValidator Validator => _validator ??= new TokenExchangeGrantValidator(
            BuildAuthClient(),
            BuildTokenValidator(),
            PermissionClient,
            NullLogger<TokenExchangeGrantValidator>.Instance);

        public Harness()
        {
            // 只替换权限查询；用户查询走 ActorPermissionClient 的真实实现，落到下面的 IAuthClient 桩
            PermissionClient = new StubActorPermissionClient(BuildAuthClient(), TokenProvider) { Owner = this };
        }

        public ExtensionGrantValidationContext CreateContext(
            string? actorToken = "actor-token",
            string? actorTokenType = OidcConstants.TokenTypeIdentifiers.AccessToken,
            string? subjectToken = null,
            string? subjectTokenType = TokenExchangeGrantValidator.SubjectTokenTypeUserId,
            bool omitSubjectToken = false,
            string? requestedTokenType = null,
            string? scope = "openid profile")
        {
            var raw = new System.Collections.Specialized.NameValueCollection();
            if (actorToken is not null)
            {
                raw["actor_token"] = actorToken;
            }
            if (actorTokenType is not null)
            {
                raw["actor_token_type"] = actorTokenType;
            }
            if (!omitSubjectToken)
            {
                raw["subject_token"] = subjectToken ?? TargetId.ToString();
            }
            if (subjectTokenType is not null)
            {
                raw["subject_token_type"] = subjectTokenType;
            }
            if (requestedTokenType is not null)
            {
                raw["requested_token_type"] = requestedTokenType;
            }
            if (scope is not null)
            {
                raw["scope"] = scope;
            }

            var client = new Client
            {
                ClientId = "masa-admin",
                AllowedGrantTypes = new[] { SdkGrantType.TOKEN_EXCHANGE },
                RequireClientSecret = RequireClientSecret
            };

            return new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Client = client,
                    GrantType = SdkGrantType.TOKEN_EXCHANGE,
                    AccessTokenLifetime = AccessTokenLifetimeSeconds,
                    Raw = raw
                }
            };
        }

        ITokenValidator BuildTokenValidator()
        {
            var proxy = DispatchProxy.Create<ITokenValidator, TokenValidatorProxy>();
            ((TokenValidatorProxy)(object)proxy).Owner = this;
            return proxy;
        }

        IAuthClient BuildAuthClient()
        {
            var proxy = DispatchProxy.Create<IAuthClient, AuthClientProxy>();
            ((AuthClientProxy)(object)proxy).Owner = this;
            return proxy;
        }

        public TokenValidationResult BuildTokenValidationResult()
        {
            if (!ActorTokenIsValid)
            {
                return new TokenValidationResult { IsError = true, Error = "invalid_token" };
            }

            var claims = new List<Claim>
            {
                new(JwtClaimTypes.Subject, ActorId.ToString()),
                new(JwtClaimTypes.ClientId, ActorTokenClientId)
            };
            claims.AddRange(ActorAuthenticationMethods.Select(m => new Claim(JwtClaimTypes.AuthenticationMethod, m)));
            if (ActorTokenHasActClaim)
            {
                claims.Add(new Claim(JwtClaimTypes.Actor, "{\"sub\":\"" + TargetId + "\"}"));
            }

            return new TokenValidationResult { IsError = false, Claims = claims };
        }
    }

    class TokenValidatorProxy : DispatchProxy
    {
        public Harness Owner { get; set; } = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
            => Task.FromResult(Owner.BuildTokenValidationResult());
    }

    /// <summary>记录权限查询的 userId（须为 actor）；<c>GetUserAsync</c> 保持真实实现。</summary>
    class StubActorPermissionClient : ActorPermissionClient
    {
        public Harness Owner { get; set; } = null!;

        public List<Guid> RequestedUserIds { get; } = new();

        public StubActorPermissionClient(IAuthClient authClient, Masa.Contrib.StackSdks.Caller.TokenProvider tokenProvider)
            : base(authClient, null!, null!, tokenProvider, NullLogger<ActorPermissionClient>.Instance) { }

        public override Task<bool> IsAuthorizedAsync(Guid userId, string permissionCode)
        {
            RequestedUserIds.Add(userId);
            return Task.FromResult(Owner.ActorAuthorized);
        }
    }

    class AuthClientProxy : DispatchProxy
    {
        public Harness Owner { get; set; } = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod?.Name == "get_UserService")
            {
                var userService = DispatchProxy.Create<IUserService, UserServiceProxy>();
                ((UserServiceProxy)(object)userService).Owner = Owner;
                return userService;
            }

            throw new Xunit.Sdk.XunitException($"Unexpected call to {targetMethod?.Name}");
        }
    }

    class UserServiceProxy : DispatchProxy
    {
        public Harness Owner { get; set; } = null!;

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            Owner.UserServiceCalls.Add(targetMethod?.Name ?? string.Empty);

            switch (targetMethod?.Name)
            {
                case nameof(IUserService.GetDetailAsync):
                    Owner.TokensSeenByGetDetail.Add(Owner.TokenProvider.AccessToken);
                    if (Owner.ThrowOnGetDetail is not null)
                    {
                        throw Owner.ThrowOnGetDetail;
                    }

                    var detailUserId = (Guid)args![0]!;
                    if (detailUserId == ActorId)
                    {
                        return Task.FromResult<Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model.UserModel?>(Owner.ActorExists
                            ? new Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model.UserModel
                            {
                                Id = ActorId,
                                Account = "actor",
                                DisplayName = "Actor Admin",
                                Enabled = Owner.ActorEnabled,
                                IsDeleted = Owner.ActorDeleted
                            }
                            : null);
                    }

                    return Task.FromResult(new Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model.UserModel
                    {
                        Id = TargetId,
                        Account = "target",
                        DisplayName = "Target User",
                        Enabled = Owner.TargetEnabled
                    });
            }

            throw new Xunit.Sdk.XunitException($"Unexpected call to {targetMethod?.Name}");
        }
    }
}
