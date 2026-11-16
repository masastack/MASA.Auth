// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Masa.Auth.Web.Sso.Infrastructure.Ldap;
using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Masa.Auth.Web.Sso.Infrastructure.Services;
using Masa.Auth.Web.Sso.Infrastructure.Validations;
using Masa.BuildingBlocks.StackSdks.Auth;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
using Masa.BuildingBlocks.StackSdks.Auth.Service;
using Masa.BuildingBlocks.StackSdks.Config;
using Masa.BuildingBlocks.StackSdks.Config.Consts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Specialized;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using Xunit;
using SdkGrantType = Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>
/// 内存 IS4 宿主上验证手工构造 validator 测不到的行为：client_secret 与 JWK 并存、省略 scope 时 offline_access 展开、
/// 生命周期上限、<c>act</c>/<c>amr</c> 的 JWT 形态，以及经 IS4 <c>ITokenValidator</c> 验签的 actor_token。
/// 经 <see cref="DefaultHttpContext"/> 走与 /connect/token 相同的认证→校验→签发链，不启 HTTP/DB/Redis。
/// </summary>
public class Is4PipelineTests
{
    const string Issuer = "https://sso.test";
    const string ClientSecret = "client-secret-value";
    static readonly Guid ActorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    static readonly Guid TargetId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    // ---------------------------------------------------------------- 客户端认证

    [Fact]
    public async Task Client_registered_with_both_a_shared_secret_and_a_jwk_still_authenticates_by_shared_secret()
    {
        using var host = new Host();

        var result = await host.RequestAsync(host.AssertionForm("domainidentity"));

        Assert.True(result.Succeeded, result.Describe());
    }

    [Fact]
    public async Task Wrong_client_secret_is_rejected_before_the_grant_is_reached()
    {
        using var host = new Host();

        var result = await host.RequestAsync(host.AssertionForm("domainidentity", clientSecret: "not-the-secret"));

        Assert.False(result.Succeeded);
        Assert.Equal("invalid_client", result.Error);
        Assert.Empty(host.Jti.Registrations);
    }

    [Fact]
    public async Task Request_without_client_credentials_is_rejected_before_the_grant_is_reached()
    {
        // validator 不再自查 client 是否为空，依赖 IS4 客户端认证
        using var host = new Host();
        var form = host.AssertionForm("domainidentity");
        form.Remove("client_id");
        form.Remove("client_secret");

        var result = await host.RequestAsync(form);

        Assert.False(result.Succeeded);
        Assert.Equal("invalid_client", result.Error);
        Assert.Empty(host.Jti.Registrations);
    }

    [Fact]
    public async Task Public_client_cannot_use_the_assertion_grant()
    {
        using var host = new Host();

        var result = await host.RequestAsync(host.AssertionForm("public-assertion", clientSecret: null));

        Assert.False(result.Succeeded);
        Assert.Empty(host.Jti.Registrations);
    }

    [Fact]
    public async Task Client_not_registered_for_the_assertion_grant_is_rejected_by_the_framework_before_the_validator_runs()
    {
        // 两个扩展 grant 的 validator 不再自查 AllowedGrantTypes，依赖 IS4 在调用 validator 前拦截
        using var host = new Host();

        var result = await host.RequestAsync(host.AssertionForm("admin-ui"));

        Assert.False(result.Succeeded);
        Assert.Equal("unsupported_grant_type", result.Error);
        Assert.Empty(host.Jti.Registrations);
    }

    [Fact]
    public async Task Client_not_registered_for_token_exchange_is_rejected_by_the_framework_before_the_validator_runs()
    {
        using var host = new Host();
        var actorToken = await host.LoginAsActorAsync();
        var form = host.ExchangeForm(actorToken, scope: "openid api1");
        form["client_id"] = "domainidentity";

        var result = await host.RequestAsync(form);

        Assert.False(result.Succeeded);
        Assert.Equal("unsupported_grant_type", result.Error);
    }

    // ---------------------------------------------------------------- jwt-bearer 断言

    [Fact]
    public async Task Assertion_grant_issues_a_jwt_with_a_single_wia_amr_and_the_domain_name()
    {
        using var host = new Host();

        var result = await host.RequestAsync(host.AssertionForm("domainidentity", scope: "openid profile api1"));

        Assert.True(result.Succeeded, result.Describe());
        var payload = Payload(result.AccessToken!);
        Assert.Equal(ActorId.ToString(), payload.GetProperty("sub").GetString());
        Assert.Equal("mayue", payload.GetProperty(IdentityClaimConsts.DOMAIN_NAME).GetString());

        // UserProfileService 会重复 Subject claims，amr 依赖 IS4 去重，否则会成 ["wia","wia"]
        Assert.Equal(new[] { "wia" }, Amr(payload));
    }

    [Fact]
    public async Task Assertion_grant_without_scope_on_an_offline_access_client_issues_a_refresh_token()
    {
        // 省略 scope 时 IS4 展开全部 scope（含 offline_access），断言通道不拦 refresh_token
        using var host = new Host(jwtClientOfflineAccess: true);

        var result = await host.RequestAsync(host.AssertionForm("domainidentity", scope: null));

        Assert.True(result.Succeeded, result.Describe());
        Assert.False(string.IsNullOrEmpty(result.RefreshToken));
    }

    [Fact]
    public async Task Assertion_grant_on_a_client_without_offline_access_never_issues_a_refresh_token()
    {
        using var host = new Host(jwtClientOfflineAccess: false);

        var result = await host.RequestAsync(host.AssertionForm("domainidentity", scope: null));

        Assert.True(result.Succeeded, result.Describe());
        Assert.True(string.IsNullOrEmpty(result.RefreshToken));
    }

    // ---------------------------------------------------------------- token-exchange 模拟

    [Fact]
    public async Task Impersonation_token_carries_act_as_a_json_object_a_single_amr_and_a_capped_lifetime()
    {
        using var host = new Host();
        var actorToken = await host.LoginAsActorAsync();

        // validator 须把客户端 3600s 收紧到 900s 并写入 exp
        var result = await host.RequestAsync(host.ExchangeForm(actorToken, scope: "openid profile api1"));

        Assert.True(result.Succeeded, result.Describe());
        Assert.Equal(900, result.AccessTokenLifetime);

        var payload = Payload(result.AccessToken!);
        Assert.Equal(TargetId.ToString(), payload.GetProperty("sub").GetString());
        Assert.Equal(900, payload.GetProperty("exp").GetInt64() - payload.GetProperty("nbf").GetInt64());

        // act 须为 JSON 对象（RFC 8693），非转义字符串
        var act = payload.GetProperty("act");
        Assert.True(act.ValueKind == JsonValueKind.Object, payload.GetRawText());
        Assert.Equal(ActorId.ToString(), act.GetProperty("sub").GetString());

        Assert.Equal(new[] { "impersonation" }, Amr(payload));

        Assert.True(string.IsNullOrEmpty(result.RefreshToken));
    }

    [Fact]
    public async Task Impersonation_without_scope_on_an_offline_access_client_is_rejected_by_the_default_scope_expansion()
    {
        // 省略 scope 展开 offline_access 时模拟禁止逻辑会触发；应关 AllowOfflineAccess 或显式传 scope
        using var host = new Host(impersonatorOfflineAccess: true);
        var actorToken = await host.LoginAsActorAsync();

        var result = await host.RequestAsync(host.ExchangeForm(actorToken, scope: null));

        Assert.False(result.Succeeded);
        Assert.Equal("invalid_grant", result.Error);
    }

    [Fact]
    public async Task Impersonation_with_an_explicit_scope_succeeds_even_if_the_client_allows_offline_access()
    {
        using var host = new Host(impersonatorOfflineAccess: true);
        var actorToken = await host.LoginAsActorAsync();

        var result = await host.RequestAsync(host.ExchangeForm(actorToken, scope: "openid api1"));

        Assert.True(result.Succeeded, result.Describe());
        Assert.True(string.IsNullOrEmpty(result.RefreshToken));
    }

    [Fact]
    public async Task Impersonation_without_scope_on_a_client_without_offline_access_succeeds()
    {
        using var host = new Host(impersonatorOfflineAccess: false);
        var actorToken = await host.LoginAsActorAsync();

        var result = await host.RequestAsync(host.ExchangeForm(actorToken, scope: null));

        Assert.True(result.Succeeded, result.Describe());
        Assert.Equal(900, result.AccessTokenLifetime);
    }

    [Fact]
    public async Task Actor_token_obtained_through_the_assertion_grant_is_rejected_by_its_wia_amr()
    {
        // 断言签出的 amr=wia 不能作为模拟的 actor_token（真实验签链路）
        using var host = new Host();
        var assertionToken = await host.RequestAsync(host.AssertionForm("domainidentity", scope: "openid profile api1"));
        Assert.True(assertionToken.Succeeded, assertionToken.Describe());

        var result = await host.RequestAsync(host.ExchangeForm(assertionToken.AccessToken!, scope: "openid api1"));

        Assert.False(result.Succeeded);
        Assert.Equal("invalid_grant", result.Error);
    }

    [Fact]
    public async Task Garbage_actor_token_is_rejected()
    {
        using var host = new Host();

        var result = await host.RequestAsync(host.ExchangeForm("not-a-token", scope: "openid api1"));

        Assert.False(result.Succeeded);
        Assert.Equal("invalid_grant", result.Error);
    }

    [Fact]
    public async Task Actor_token_signed_by_a_foreign_key_is_rejected()
    {
        // 异宿主密钥签出的合法 token 应被拒
        using var foreign = new Host();
        var foreignToken = await foreign.LoginAsActorAsync();
        using var host = new Host();

        var result = await host.RequestAsync(host.ExchangeForm(foreignToken, scope: "openid api1"));

        Assert.False(result.Succeeded);
        Assert.Equal("invalid_grant", result.Error);
    }

    // ---------------------------------------------------------------- 工具

    /// <summary>amr 在 IS4 签发的 JWT 里是数组；兼容单值字符串写法，便于断言"恰好一个值"。</summary>
    static string[] Amr(JsonElement payload)
    {
        var amr = payload.GetProperty("amr");
        return amr.ValueKind == JsonValueKind.Array
            ? amr.EnumerateArray().Select(x => x.GetString()!).ToArray()
            : new[] { amr.GetString()! };
    }

    static JsonElement Payload(string jwt)
    {
        var json = Base64UrlEncoder.Decode(jwt.Split('.')[1]);
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    sealed record TokenResult(
        bool Succeeded, string? Error, string? Description, string? AccessToken, string? RefreshToken, int AccessTokenLifetime)
    {
        public string Describe() => $"{Error}: {Description}";
    }

    /// <summary>内存 IS4 宿主；桩只替换进程外 I/O，不替换管线组件与 validator。</summary>
    sealed class Host : IDisposable
    {
        readonly ServiceProvider _provider;
        readonly RSA _callerKey = RSA.Create(2048);
        readonly HttpContextAccessor _accessor = new();

        public StubJti Jti { get; } = new();

        public Host(bool jwtClientOfflineAccess = false, bool impersonatorOfflineAccess = false)
        {
            var publicJwk = JsonSerializer.Serialize(JsonWebKeyConverter.ConvertFromRSASecurityKey(
                new RsaSecurityKey(_callerKey.ExportParameters(false)) { KeyId = "domainidentity-k1" }));

            Client NewClient(string id, string grantType, bool offline, bool requireSecret = true, params Secret[] extraSecrets)
            {
                var client = new Client
                {
                    ClientId = id,
                    AllowedGrantTypes = new[] { grantType },
                    AllowedScopes = { "openid", "profile", "api1" },
                    AllowOfflineAccess = offline,
                    AccessTokenLifetime = 3600,
                    RequireClientSecret = requireSecret
                };
                if (requireSecret)
                {
                    client.ClientSecrets.Add(new Secret(ClientSecret.Sha256()));
                }

                foreach (var secret in extraSecrets)
                {
                    client.ClientSecrets.Add(secret);
                }

                return client;
            }

            var clients = new[]
            {
                // SharedSecret + JWK 并存（域身份客户端形态）
                NewClient("domainidentity", SdkGrantType.JWT_BEARER, jwtClientOfflineAccess, true,
                    new Secret { Type = IdentityServerConstants.SecretTypes.JsonWebKey, Value = publicJwk }),
                NewClient("public-assertion", SdkGrantType.JWT_BEARER, false, requireSecret: false,
                    new Secret { Type = IdentityServerConstants.SecretTypes.JsonWebKey, Value = publicJwk }),
                NewClient("admin-ui", "test_login", false),
                NewClient("impersonator", SdkGrantType.TOKEN_EXCHANGE, impersonatorOfflineAccess)
            };

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAuthentication();
            services.AddSingleton<IHttpContextAccessor>(_accessor);
            var authClient = BuildAuthClient();
            services.AddSingleton(authClient);
            services.AddSingleton<DomainAccountResolver>(new StubResolver());
            services.AddSingleton<JtiReplayCache>(Jti);
            services.AddSingleton<ActorPermissionClient>(new StubActorPermission(authClient));
            services.AddSingleton<IMasaStackConfig>(StubMasaStackConfig.WithSsoDomain(Issuer));

            services.AddIdentityServer(options => options.IssuerUri = Issuer)
                .AddInMemoryIdentityResources(new IdentityResource[] { new IdentityResources.OpenId(), new IdentityResources.Profile() })
                .AddInMemoryApiScopes(new[] { new ApiScope("api1") })
                .AddInMemoryClients(clients)
                // 与生产一致：ProfileService 会重复发 Subject claims
                .AddProfileService<UserProfileService>()
                .AddDeveloperSigningCredential(persistKey: false)
                .AddExtensionGrantValidator<TestLoginGrantValidator>()
                .AddExtensionGrantValidator<JwtBearerAssertionGrantValidator>()
                .AddExtensionGrantValidator<TokenExchangeGrantValidator>();

            // 与 Program.cs 一致：避免 JSON claim（act）写成嵌套数组
            services.Replace(ServiceDescriptor.Transient<ITokenCreationService, JsonClaimTokenCreationService>());

            _provider = services.BuildServiceProvider();
        }

        public Dictionary<string, string?> AssertionForm(
            string clientId, string? clientSecret = ClientSecret, string? scope = "openid profile api1")
        {
            var form = new Dictionary<string, string?>
            {
                ["grant_type"] = SdkGrantType.JWT_BEARER,
                ["client_id"] = clientId,
                ["assertion"] = CreateAssertion(clientId)
            };
            if (clientSecret is not null)
            {
                form["client_secret"] = clientSecret;
            }

            if (scope is not null)
            {
                form["scope"] = scope;
            }

            return form;
        }

        public Dictionary<string, string?> ExchangeForm(string actorToken, string? scope)
        {
            var form = new Dictionary<string, string?>
            {
                ["grant_type"] = SdkGrantType.TOKEN_EXCHANGE,
                ["client_id"] = "impersonator",
                ["client_secret"] = ClientSecret,
                ["actor_token"] = actorToken,
                ["actor_token_type"] = IdentityModel.OidcConstants.TokenTypeIdentifiers.AccessToken,
                ["subject_token"] = TargetId.ToString(),
                ["subject_token_type"] = TokenExchangeGrantValidator.SubjectTokenTypeUserId
            };
            if (scope is not null)
            {
                form["scope"] = scope;
            }

            return form;
        }

        /// <summary>经真实管线以"密码登录"（amr=pwd）为管理员签出 access token，作为 actor_token 使用。</summary>
        public async Task<string> LoginAsActorAsync()
        {
            var result = await RequestAsync(new Dictionary<string, string?>
            {
                ["grant_type"] = "test_login",
                ["client_id"] = "admin-ui",
                ["client_secret"] = ClientSecret,
                ["scope"] = "openid profile api1"
            });
            Assert.True(result.Succeeded, result.Describe());
            return result.AccessToken!;
        }

        string CreateAssertion(string clientId)
        {
            var now = DateTime.UtcNow;
            return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
            {
                Issuer = clientId,
                Audience = $"{Issuer}/connect/token",
                Subject = new ClaimsIdentity(new[] { new Claim("sub", "LONSID\\mayue") }),
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddSeconds(60),
                Claims = new Dictionary<string, object> { ["jti"] = Guid.NewGuid().ToString("N") },
                SigningCredentials = new SigningCredentials(
                    new RsaSecurityKey(_callerKey) { KeyId = "domainidentity-k1" },
                    SecurityAlgorithms.RsaSha256)
            });
        }

        /// <summary>与 /connect/token 端点同一条链：客户端认证 → 请求校验 → 响应生成。</summary>
        public async Task<TokenResult> RequestAsync(Dictionary<string, string?> form)
        {
            using var scope = _provider.CreateScope();
            var services = scope.ServiceProvider;

            // IS4 扩展方法从 HttpContext.RequestServices 取依赖
            var context = new DefaultHttpContext { RequestServices = services };
            context.Request.Scheme = "https";
            context.Request.Host = new HostString("sso.test");
            context.Request.Method = "POST";
            context.Request.ContentType = "application/x-www-form-urlencoded";
            context.Request.Form = new FormCollection(form.ToDictionary(
                pair => pair.Key, pair => new StringValues(pair.Value)));
            _accessor.HttpContext = context;

            var clientResult = await services.GetRequiredService<IClientSecretValidator>().ValidateAsync(context);
            if (clientResult.IsError)
            {
                // IS4 4.x 仅置 IsError，invalid_client 由端点补全
                return new TokenResult(false, clientResult.Error ?? "invalid_client", clientResult.ErrorDescription, null, null, 0);
            }

            var raw = new NameValueCollection();
            foreach (var pair in form)
            {
                raw[pair.Key] = pair.Value;
            }

            var validation = await services.GetRequiredService<ITokenRequestValidator>()
                .ValidateRequestAsync(raw, clientResult);
            if (validation.IsError)
            {
                return new TokenResult(false, validation.Error, validation.ErrorDescription, null, null, 0);
            }

            var response = await services.GetRequiredService<ITokenResponseGenerator>().ProcessAsync(validation);
            return new TokenResult(true, null, null, response.AccessToken, response.RefreshToken, response.AccessTokenLifetime);
        }

        IAuthClient BuildAuthClient() => DispatchProxy.Create<IAuthClient, AuthClientProxy>();

        public void Dispose()
        {
            _provider.Dispose();
            _callerKey.Dispose();
        }
    }

    sealed class TestLoginGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "test_login";

        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            // test_login：amr=pwd，与断言 wia 区分
            context.Result = new GrantValidationResult(ActorId.ToString(), "pwd");
            return Task.CompletedTask;
        }
    }

    sealed class StubJti : JtiReplayCache
    {
        public StubJti() : base(null!, NullLogger<JtiReplayCache>.Instance) { }

        public List<string> Registrations { get; } = new();

        public override Task<bool> TryRegisterAsync(string clientId, string jti, DateTime expiresUtc, TimeSpan clockSkew = default)
        {
            Registrations.Add(jti);
            return Task.FromResult(true);
        }
    }

    sealed class StubResolver : DomainAccountResolver
    {
        public StubResolver() : base(null!, null!, null!, NullLogger<DomainAccountResolver>.Instance) { }

        public override Task<ResolvedAccount?> ResolveAsync(string account)
            => Task.FromResult<ResolvedAccount?>(new ResolvedAccount("mayue", "ad-guid-1", IsDisabled: false));
    }

    /// <summary>只替换权限查询；用户查询走真实 <c>GetUserAsync</c>，落到宿主里的 <see cref="IAuthClient"/> 桩。</summary>
    sealed class StubActorPermission : ActorPermissionClient
    {
        public StubActorPermission(IAuthClient authClient)
            : base(authClient, null!, null!, new Masa.Contrib.StackSdks.Caller.TokenProvider(), NullLogger<ActorPermissionClient>.Instance) { }

        public override Task<bool> IsAuthorizedAsync(Guid userId, string permissionCode) => Task.FromResult(true);
    }

    class AuthClientProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod?.Name == "get_UserService")
            {
                return DispatchProxy.Create<IUserService, UserServiceProxy>();
            }

            // 非本测试范围的 Admin 调用；BaseGrantValidator 会忽略失败
            throw new InvalidOperationException($"Unexpected call to {targetMethod?.Name}");
        }
    }

    class UserServiceProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            switch (targetMethod?.Name)
            {
                case nameof(IUserService.GetThirdPartyUserAsync):
                    // AD 账号映射到 ActorId
                    return Task.FromResult<UserModel?>(User(ActorId, "mayue"));

                case nameof(IUserService.GetDetailAsync):
                    var id = (Guid)args![0]!;
                    return Task.FromResult<UserModel?>(User(id, id == ActorId ? "mayue" : "target"));

                case nameof(IUserService.GetClaimValuesAsync):
                    // 无自定义 claim
                    return EmptyTask(targetMethod);
            }

            throw new InvalidOperationException($"Unexpected call to {targetMethod?.Name}");
        }

        /// <summary>返回 <c>Task&lt;T&gt;</c> 的空实例（T 取方法的实际返回类型，避免测试与 SDK 的集合类型耦合）。</summary>
        static object EmptyTask(MethodInfo method)
        {
            var resultType = method.ReturnType.GetGenericArguments()[0];
            var empty = Activator.CreateInstance(resultType)!;
            return typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { empty })!;
        }

        static UserModel User(Guid id, string account) => new()
        {
            Id = id,
            Account = account,
            DisplayName = account,
            Enabled = true
        };
    }
}
