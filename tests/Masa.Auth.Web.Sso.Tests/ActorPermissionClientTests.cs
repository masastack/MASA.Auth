// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License, Version 2.0.

using System.Reflection;
using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Masa.BuildingBlocks.Authentication.Identity;
using Masa.BuildingBlocks.Service.Caller;
using Masa.BuildingBlocks.StackSdks.Auth;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
using Masa.BuildingBlocks.StackSdks.Auth.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using SdkPermissionService = Masa.Contrib.StackSdks.Auth.Service.PermissionService;
using TokenProvider = Masa.Contrib.StackSdks.Caller.TokenProvider;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>
/// <see cref="ActorPermissionClient"/> 测试：/connect/token 没有已认证用户，客户端必须靠 <see cref="IUserSetter"/>
/// 把当前用户临时切到 actor。这里用真实的 <c>AddMasaIdentity()</c> 与真实的 SDK <c>PermissionService</c>，
/// 只替换最底层的 <see cref="ICaller"/>，因此能证明"SDK 确实从 IUserContext 取 userId 并发给 Admin"。
/// 用户查询（<c>GetUserAsync</c>）则验证 actor_token 只在调用期间作为 Bearer 来源，之后恢复入站请求原有的 token。
/// </summary>
public class ActorPermissionClientTests
{
    const string AuthServiceId = "masa-auth-service-admin";
    const string PermissionCode = "api.user.impersonate";

    /// <summary>模拟入站请求自带的 Authorization（/connect/token 上通常是空或 Basic）；调用后必须恢复成它。</summary>
    const string InboundToken = "inbound-authorization";

    static readonly Guid ActorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    static readonly Guid TargetId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task Sdk_request_carries_the_actor_as_user_and_the_auth_service_id()
    {
        using var harness = new Harness();

        var authorized = await harness.Client.IsAuthorizedAsync(ActorId, PermissionCode);

        Assert.True(authorized);
        Assert.Equal(
            $"api/permission/authorized?appId={AuthServiceId}&code={PermissionCode}&userId={ActorId}",
            Assert.Single(harness.Caller.Urls));
    }

    [Fact]
    public async Task Current_user_is_the_actor_during_the_check_and_restored_afterwards()
    {
        using var harness = new Harness();
        Assert.Equal(Guid.Empty, harness.UserContext.GetUserId<Guid>());

        await harness.Client.IsAuthorizedAsync(ActorId, PermissionCode);

        Assert.False(harness.UserContext.IsAuthenticated);
        Assert.Equal(Guid.Empty, harness.UserContext.GetUserId<Guid>());
    }

    [Fact]
    public async Task False_from_the_sdk_is_a_denial()
    {
        using var harness = new Harness { Caller = { Result = false } };

        Assert.False(await harness.Client.IsAuthorizedAsync(ActorId, PermissionCode));
    }

    [Fact]
    public async Task Sdk_failure_is_a_denial_and_still_restores_the_current_user()
    {
        using var harness = new Harness { Caller = { Failure = new HttpRequestException("http://10.0.0.7:8080 connection refused") } };

        var authorized = await harness.Client.IsAuthorizedAsync(ActorId, PermissionCode);

        Assert.False(authorized);
        Assert.Single(harness.Caller.Urls);
        Assert.False(harness.UserContext.IsAuthenticated);
    }

    [Fact]
    public async Task User_lookup_carries_the_actor_token_and_restores_the_inbound_one()
    {
        using var harness = new Harness();

        var user = await harness.Client.GetUserAsync(TargetId, "the-actor-token");

        Assert.Equal(TargetId, user!.Id);
        Assert.Equal("the-actor-token", Assert.Single(harness.Users.TokensSeen));
        Assert.Equal(InboundToken, harness.TokenProvider.AccessToken);
    }

    [Fact]
    public async Task User_lookup_failure_propagates_and_still_restores_the_inbound_token()
    {
        // 与权限查询不同：用户查询不吞异常，由 validator 按拒绝处理
        using var harness = new Harness { Users = { Failure = new HttpRequestException("401 Unauthorized") } };

        await Assert.ThrowsAsync<HttpRequestException>(() => harness.Client.GetUserAsync(TargetId, "the-actor-token"));

        Assert.Single(harness.Users.TokensSeen);
        Assert.Equal(InboundToken, harness.TokenProvider.AccessToken);
    }

    [Fact]
    public async Task Permission_check_does_not_touch_the_forwarded_token()
    {
        // 权限查询靠 IUserSetter 提供 userId，不需要也不改 Bearer
        using var harness = new Harness();

        await harness.Client.IsAuthorizedAsync(ActorId, PermissionCode);

        Assert.Equal(InboundToken, harness.TokenProvider.AccessToken);
        Assert.Empty(harness.Users.TokensSeen);
    }

    sealed class Harness : IDisposable
    {
        readonly ServiceProvider _provider;
        readonly IServiceScope _scope;

        public StubCaller Caller { get; }

        /// <summary>SDK 转发给 Auth 服务的 Bearer 来源；初始值模拟入站请求自带的 Authorization。</summary>
        public TokenProvider TokenProvider { get; } = new() { AccessToken = InboundToken };

        public StubUserService Users { get; }

        public ActorPermissionClient Client { get; }

        public IUserContext UserContext { get; }

        public Harness()
        {
            var caller = DispatchProxy.Create<ICaller, StubCaller>();
            Caller = (StubCaller)(object)caller;

            var userService = DispatchProxy.Create<IUserService, StubUserService>();
            Users = (StubUserService)(object)userService;
            Users.TokenProvider = TokenProvider;

            var services = new ServiceCollection();
            services.AddLogging();
            // 与 Program.cs 一致：AddMasaIdentity 注册的 IUserSetter 与 IUserContext 是同一个 scoped 实例
            services.AddMasaIdentity();
            // 与生产的 AuthClient 一致：SDK 的 PermissionService 持有 IMultiEnvironmentUserContext
            services.AddScoped<IAuthClient>(sp => AuthClientProxy.For(
                new SdkPermissionService(caller, sp.GetRequiredService<IMultiEnvironmentUserContext>()),
                userService));
            services.AddScoped(sp => new ActorPermissionClient(
                sp.GetRequiredService<IAuthClient>(),
                sp.GetRequiredService<IUserSetter>(),
                StubMasaStackConfig.WithAuthServiceId(AuthServiceId),
                TokenProvider,
                NullLogger<ActorPermissionClient>.Instance));

            _provider = services.BuildServiceProvider();
            _scope = _provider.CreateScope();
            Client = _scope.ServiceProvider.GetRequiredService<ActorPermissionClient>();
            UserContext = _scope.ServiceProvider.GetRequiredService<IUserContext>();
        }

        public void Dispose()
        {
            _scope.Dispose();
            _provider.Dispose();
        }
    }

    /// <summary>只实现 SDK <c>PermissionService.AuthorizedAsync</c> 用到的 <c>GetAsync&lt;T&gt;(url)</c>，并记录请求地址。</summary>
    public class StubCaller : DispatchProxy
    {
        public List<string> Urls { get; } = new();

        public bool Result { get; set; } = true;

        public Exception? Failure { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod is { Name: "GetAsync", IsGenericMethod: true } && args is { Length: > 0 } && args[0] is string url)
            {
                Urls.Add(url);
                if (Failure is not null)
                {
                    throw Failure;
                }

                var resultType = targetMethod.ReturnType.GetGenericArguments()[0];
                var value = resultType == typeof(bool) ? Result : Activator.CreateInstance(resultType);
                return typeof(Task).GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(resultType)
                    .Invoke(null, new[] { value });
            }

            throw new InvalidOperationException($"Unexpected call to {targetMethod?.Name}");
        }
    }

    /// <summary>只实现 <c>GetDetailAsync(Guid)</c>：记录调用瞬间 <see cref="TokenProvider"/> 上的 token，可注入失败。</summary>
    public class StubUserService : DispatchProxy
    {
        public TokenProvider TokenProvider { get; set; } = null!;

        public List<string?> TokensSeen { get; } = new();

        public Exception? Failure { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            if (targetMethod?.Name == nameof(IUserService.GetDetailAsync) && args is { Length: 1 } && args[0] is Guid userId)
            {
                TokensSeen.Add(TokenProvider.AccessToken);
                if (Failure is not null)
                {
                    throw Failure;
                }

                return Task.FromResult<UserModel?>(new UserModel { Id = userId });
            }

            throw new InvalidOperationException($"Unexpected call to {targetMethod?.Name}");
        }
    }

    /// <summary><see cref="IAuthClient"/> 只需要 <c>PermissionService</c> 与 <c>UserService</c>；其余成员不应被访问。</summary>
    public class AuthClientProxy : DispatchProxy
    {
        IPermissionService? _permissionService;
        IUserService? _userService;

        public static IAuthClient For(IPermissionService permissionService, IUserService userService)
        {
            var proxy = Create<IAuthClient, AuthClientProxy>();
            var self = (AuthClientProxy)(object)proxy;
            self._permissionService = permissionService;
            self._userService = userService;
            return proxy;
        }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
            => targetMethod?.Name switch
            {
                "get_" + nameof(IAuthClient.PermissionService) => _permissionService,
                "get_" + nameof(IAuthClient.UserService) => _userService,
                _ => throw new InvalidOperationException($"Unexpected call to {targetMethod?.Name}")
            };
    }
}
