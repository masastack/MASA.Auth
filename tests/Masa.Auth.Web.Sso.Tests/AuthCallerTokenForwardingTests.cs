// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Web.Sso.Infrastructure;
using Masa.Auth.Web.Sso.Infrastructure.Oidc;
using Masa.BuildingBlocks.Authentication.Identity;
using Masa.BuildingBlocks.Service.Caller;
using Masa.Contrib.StackSdks.Isolation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Net.Http.Headers;
using Xunit;
using SdkUserService = Masa.Contrib.StackSdks.Auth.Service.UserService;
using TokenProvider = Masa.Contrib.StackSdks.Caller.TokenProvider;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>
/// 模拟登录靠"调用期间改写请求作用域内的 <see cref="TokenProvider"/>"让 SDK 把 actor_token 作为 Bearer 发给 Auth 服务。
/// 这里用 SDK 自己的 Caller 组件（与 <c>AddAuthClient</c> 相同的 <c>UseHttpClient</c> + <c>UseAuthentication()</c> 接线，
/// 只是去掉 Redis 多级缓存）验证该前提：调用方先创建、之后再改 token 也会生效；不同作用域互不影响。
/// </summary>
public class AuthCallerTokenForwardingTests
{
    const string CallerName = "test.auth.caller";
    const string Url = "api/user/byId/22222222-2222-2222-2222-222222222222";

    [Fact]
    public async Task Token_set_after_the_caller_was_created_is_sent_as_Bearer_and_cleared_token_sends_none()
    {
        var handler = new CapturingHandler();
        using var provider = BuildProvider(handler);
        using var scope = provider.CreateScope();

        // 与生产顺序一致：IAuthClient（含 caller）先于 validator.ValidateAsync 创建
        var caller = scope.ServiceProvider.GetRequiredService<ICallerFactory>().Create(CallerName);
        var tokenProvider = scope.ServiceProvider.GetRequiredService<TokenProvider>();

        await caller.GetAsync<Dictionary<string, object>>(Url);
        Assert.Null(handler.LastAuthorization);

        tokenProvider.AccessToken = "the-actor-token";
        await caller.GetAsync<Dictionary<string, object>>(Url);
        Assert.Equal(new AuthenticationHeaderValue("Bearer", "the-actor-token"), handler.LastAuthorization);

        tokenProvider.AccessToken = null;
        await caller.GetAsync<Dictionary<string, object>>(Url);
        Assert.Null(handler.LastAuthorization);
    }

    [Fact]
    public async Task Token_set_in_one_scope_does_not_leak_into_another_scope()
    {
        var handler = new CapturingHandler();
        using var provider = BuildProvider(handler);

        using (var first = provider.CreateScope())
        {
            first.ServiceProvider.GetRequiredService<TokenProvider>().AccessToken = "the-actor-token";
        }

        using var second = provider.CreateScope();
        var caller = second.ServiceProvider.GetRequiredService<ICallerFactory>().Create(CallerName);
        await caller.GetAsync<Dictionary<string, object>>(Url);

        Assert.Null(handler.LastAuthorization);
    }

    [Fact]
    public async Task GetUserAsync_puts_the_actor_token_on_the_wire_in_place_of_the_inbound_Basic_credentials()
    {
        // 与 Program.cs 的 AddAuthClient 相同的 caller 接线（EnvironmentCallerMiddleware + UseAuthentication），
        // 外加 Program.cs 里挂在同一个 HttpClient 上的 CultureForwardingHandler；再走真实的 SDK UserService
        const string InboundBasic = "Basic Y2xpZW50OnNlY3JldA==";
        var handler = new CapturingHandler();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMasaIdentity();
        services.AddCaller(CallerName, callerBuilder => callerBuilder
            .UseHttpClient(client => client.BaseAddress = "https://auth.test")
            .AddMiddleware<EnvironmentCallerMiddleware>()
            .UseAuthentication());
        services.Configure<HttpClientFactoryOptions>(CallerName, options =>
        {
            options.HttpMessageHandlerBuilderActions.Add(builder => builder.AdditionalHandlers.Add(
                new CultureForwardingHandler(builder.Services.GetRequiredService<IHttpContextAccessor>())));
            options.HttpMessageHandlerBuilderActions.Add(builder => builder.PrimaryHandler = handler);
        });
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // /connect/token 请求：客户端认证用 Basic，没有 Bearer
        var httpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        httpContext.Request.Headers.Authorization = InboundBasic;
        httpContext.Request.Headers.AcceptLanguage = "zh-CN";
        scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext = httpContext;

        var tokenProvider = scope.ServiceProvider.GetRequiredService<TokenProvider>();
        var caller = scope.ServiceProvider.GetRequiredService<ICallerFactory>().Create(CallerName);
        var authClient = ActorPermissionClientTests.AuthClientProxy.For(
            null!,
            new SdkUserService(caller, scope.ServiceProvider.GetRequiredService<IUserContext>(), null!));
        var client = new ActorPermissionClient(
            authClient,
            scope.ServiceProvider.GetRequiredService<IUserSetter>(),
            StubMasaStackConfig.WithAuthServiceId("masa-auth-service-admin"),
            tokenProvider,
            NullLogger<ActorPermissionClient>.Instance);
        Assert.Equal(InboundBasic, tokenProvider.AccessToken);

        await client.GetUserAsync(TargetId, "the-actor-token");

        Assert.Equal($"/{Url}", handler.LastUri!.AbsolutePath);
        Assert.Equal(new AuthenticationHeaderValue("Bearer", "the-actor-token"), handler.LastAuthorization);
        Assert.Contains("zh-CN", handler.LastAcceptLanguage);
        Assert.Equal(InboundBasic, tokenProvider.AccessToken);
    }

    static readonly Guid TargetId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    static ServiceProvider BuildProvider(CapturingHandler handler)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddCaller(CallerName, callerBuilder => callerBuilder
            .UseHttpClient(client => client.BaseAddress = "https://auth.test")
            .UseAuthentication());
        services.Configure<HttpClientFactoryOptions>(CallerName, options =>
            options.HttpMessageHandlerBuilderActions.Add(builder => builder.PrimaryHandler = handler));
        return services.BuildServiceProvider();
    }

    sealed class CapturingHandler : HttpMessageHandler
    {
        public AuthenticationHeaderValue? LastAuthorization { get; private set; }

        public Uri? LastUri { get; private set; }

        public string LastAcceptLanguage { get; private set; } = string.Empty;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastAuthorization = request.Headers.Authorization;
            LastUri = request.RequestUri;
            LastAcceptLanguage = request.Headers.AcceptLanguage.ToString();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
            });
        }
    }
}
