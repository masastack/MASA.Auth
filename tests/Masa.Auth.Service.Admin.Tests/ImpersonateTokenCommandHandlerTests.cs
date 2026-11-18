// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Net;
using System.Text;
using Masa.Auth.Contracts.Admin;
using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Masa.Auth.Domain.Logs.Repositories;
using Masa.Auth.Domain.Subjects.Events;
using Masa.Auth.Service.Admin.Application.Subjects;
using Masa.Auth.Service.Admin.Application.Subjects.Commands;
using Masa.Auth.Service.Admin.Infrastructure.Models;
using Masa.BuildingBlocks.Authentication.Identity;
using Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans;
using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.BuildingBlocks.StackSdks.Config;
using Masa.Contrib.StackSdks.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// 模拟登录代理（ImpersonateTokenCommandHandler）判定语义：自我模拟/无权限/未配置/缺 Bearer 各自独立拒绝，
/// 权限查询异常 fail-closed；成功时透传 SSO 的 access_token/expires_in 并记操作日志；
/// 请求表单符合 RFC 8693 token-exchange 约定（grant/actor/subject/type/scope）。
/// 全部依赖走桩（StubProxy + 假 HttpMessageHandler），不依赖数据库/Redis/真实 SSO。
/// </summary>
public class ImpersonateTokenCommandHandlerTests
{
    static readonly Guid Actor = new(1, 0, 0, new byte[8]);
    static readonly Guid Target = new(2, 0, 0, new byte[8]);

    [Fact]
    public async Task Happy_path_exchanges_token_and_records_operation_log()
    {
        var harness = new Harness();

        var command = new ImpersonateTokenCommand(Target);
        await harness.NewHandler().ImpersonateTokenAsync(command);

        Assert.Equal("impersonated-token", command.Result.AccessToken);
        Assert.Equal(900, command.Result.ExpiresIn);

        var log = Assert.Single(harness.OperationLogs);
        Assert.Equal("Login", log.Type);
        Assert.Contains(Actor.ToString(), log.Description);
        Assert.Contains(Target.ToString(), log.Description);
        Assert.Equal(Actor.ToString(), log.Operator);

        Assert.EndsWith("/connect/token", harness.Http.LastUrl);
        var form = harness.LastForm;
        Assert.Equal(GrantType.TOKEN_EXCHANGE, form["grant_type"]);
        Assert.Equal("masa-admin-impersonator", form["client_id"]);
        Assert.Equal("shared-secret", form["client_secret"]);
        Assert.Equal("actor-token", form["actor_token"]);
        Assert.Equal("urn:ietf:params:oauth:token-type:access_token", form["actor_token_type"]);
        Assert.Equal(Target.ToString(), form["subject_token"]);
        Assert.Equal(ImpersonationConsts.SubjectTokenTypeUserId, form["subject_token_type"]);
        Assert.Equal("openid profile", form["scope"]);
    }

    [Fact]
    public async Task Configured_scope_is_sent_instead_of_a_hard_coded_one()
    {
        var harness = new Harness { Scope = "openid profile auth-service" };

        await harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Target));

        Assert.Equal("openid profile auth-service", harness.LastForm["scope"]);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Blank_scope_falls_back_to_the_default_and_is_never_omitted(string? configured)
    {
        // token-exchange 必须显式带 scope：省略会被 IdentityServer 展开成客户端全部 scope（可能含 offline_access）
        var harness = new Harness { Scope = configured };

        await harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Target));

        Assert.Equal(ImpersonationClientOptions.DefaultScope, harness.LastForm["scope"]);
    }

    [Fact]
    public async Task Configured_scope_is_normalised_for_whitespace_and_duplicates()
    {
        var harness = new Harness { Scope = "  openid   profile  openid " };

        await harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Target));

        Assert.Equal("openid profile", harness.LastForm["scope"]);
    }

    [Fact]
    public async Task Self_impersonation_is_rejected_before_any_call()
    {
        var harness = new Harness();

        var ex = await Assert.ThrowsAsync<UserFriendlyException>(
            () => harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Actor)));

        Assert.Equal(UserFriendlyExceptionCodes.IMPERSONATION_SELF_NOT_ALLOWED, ex.ErrorCode);
        Assert.Null(harness.Http.LastUrl);
        Assert.Empty(harness.OperationLogs);
    }

    [Fact]
    public async Task Unauthorized_actor_is_rejected_without_calling_sso()
    {
        var harness = new Harness { Authorized = false };

        var ex = await Assert.ThrowsAsync<UserFriendlyException>(
            () => harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Target)));

        Assert.Equal(UserFriendlyExceptionCodes.IMPERSONATION_NOT_AUTHORIZED, ex.ErrorCode);
        Assert.Null(harness.Http.LastUrl);
    }

    [Fact]
    public async Task Permission_check_failure_is_denied_fail_closed()
    {
        var harness = new Harness { EventBusError = new InvalidOperationException("permission backend down") };

        var ex = await Assert.ThrowsAsync<UserFriendlyException>(
            () => harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Target)));

        Assert.Equal(UserFriendlyExceptionCodes.IMPERSONATION_NOT_AUTHORIZED, ex.ErrorCode);
        Assert.Null(harness.Http.LastUrl);
    }

    [Fact]
    public async Task Missing_client_credentials_is_rejected()
    {
        var harness = new Harness { ClientId = null };

        var ex = await Assert.ThrowsAsync<UserFriendlyException>(
            () => harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Target)));

        Assert.Equal(UserFriendlyExceptionCodes.IMPERSONATION_CLIENT_NOT_CONFIGURED, ex.ErrorCode);
    }

    [Fact]
    public async Task Sso_rejection_is_surfaced_as_token_exchange_failure()
    {
        var harness = new Harness
        {
            Status = HttpStatusCode.BadRequest,
            Body = "{\"error\":\"impersonation_not_authorized\",\"error_description\":\"无权模拟该用户\"}"
        };

        var ex = await Assert.ThrowsAsync<UserFriendlyException>(
            () => harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Target)));

        Assert.Equal(UserFriendlyExceptionCodes.IMPERSONATION_TOKEN_EXCHANGE_FAILED, ex.ErrorCode);
        Assert.Empty(harness.OperationLogs);
    }

    [Fact]
    public async Task Success_response_without_access_token_is_rejected()
    {
        var harness = new Harness { Body = "{\"expires_in\":900}" };

        var ex = await Assert.ThrowsAsync<UserFriendlyException>(
            () => harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Target)));

        Assert.Equal(UserFriendlyExceptionCodes.IMPERSONATION_TOKEN_EXCHANGE_FAILED, ex.ErrorCode);
    }

    [Fact]
    public async Task Missing_bearer_token_is_rejected()
    {
        var harness = new Harness { Bearer = null };

        var ex = await Assert.ThrowsAsync<UserFriendlyException>(
            () => harness.NewHandler().ImpersonateTokenAsync(new ImpersonateTokenCommand(Target)));

        Assert.Equal(UserFriendlyExceptionCodes.USER_NOT_EXIST, ex.ErrorCode);
        Assert.Null(harness.Http.LastUrl);
    }

    sealed class Harness
    {
        public bool Authorized = true;
        public Exception? EventBusError;
        public HttpStatusCode Status = HttpStatusCode.OK;
        public string Body = "{\"access_token\":\"impersonated-token\",\"expires_in\":900}";
        public string? Bearer = "actor-token";
        public string? ClientId = "masa-admin-impersonator";
        public string? ClientSecret = "shared-secret";
        public string? Scope;

        public RecordingHttpHandler Http { get; } = new();
        public List<(string Type, string Description, string? Operator)> OperationLogs { get; } = new();

        public Harness()
        {
            //延迟读取 Status/Body：测试可在 NewHandler 前改写这些字段
            Http.Responder = () => new HttpResponseMessage(Status)
            {
                Content = new StringContent(Body, Encoding.UTF8, "application/json")
            };
        }

        public ImpersonateTokenCommandHandler NewHandler()
        {
            var eventBus = StubProxy.Create<IEventBus>((method, args) =>
            {
                if (method.Name != nameof(IEventBus.PublishAsync))
                    return (false, null);

                if (EventBusError is not null)
                    return (true, Task.FromException(EventBusError));

                if (args![0] is UserAuthorizedDomainEvent authorized)
                    authorized.Authorized = Authorized;
                return (true, Task.CompletedTask);
            });

            var userContext = StubProxy.Create<IUserContext>((method, _) =>
                method.Name == nameof(IUserContext.GetUserId) ? (true, (object)Actor) : (false, null));

            var httpClientFactory = StubProxy.Create<IHttpClientFactory>((method, _) =>
                method.Name == nameof(IHttpClientFactory.CreateClient)
                    //生产环境的 SsoAddr 命名客户端带 SSO BaseAddress；相对路径 /connect/token 依赖它
                    ? (true, (object)new HttpClient(Http) { BaseAddress = new Uri("https://sso.example.com") })
                    : (false, null));

            var masaStackConfig = StubProxy.Create<IMasaStackConfig>((method, _) =>
                method.Name == "GetServiceId" ? (true, (object)"masa-auth-service") : (false, null));

            var httpContextAccessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
            if (Bearer is not null)
                httpContextAccessor.HttpContext.Request.Headers.Authorization = "Bearer " + Bearer;

            var operationLogRepository = StubProxy.Create<IOperationLogRepository>((method, args) =>
            {
                if (method.Name != nameof(IOperationLogRepository.AddDefaultAsync))
                    return (false, null);

                OperationLogs.Add((
                    args![0]!.ToString() ?? string.Empty,
                    (string)args[1]!,
                    args.Length > 2 ? args[2]?.ToString() : null));
                return (true, Task.CompletedTask);
            });

            var options = Options.Create(new ImpersonationClientOptions
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Scope = Scope
            });

            return new ImpersonateTokenCommandHandler(
                userContext,
                eventBus,
                httpClientFactory,
                masaStackConfig,
                httpContextAccessor,
                operationLogRepository,
                options,
                NullLogger<ImpersonateTokenCommandHandler>.Instance);
        }

        public Dictionary<string, string> LastForm => (Http.LastFormRaw ?? string.Empty)
            .Split('&', StringSplitOptions.RemoveEmptyEntries)
            .Select(pair => pair.Split('=', 2))
            .ToDictionary(
                //application/x-www-form-urlencoded 中空格编码为 '+'，Uri.UnescapeDataString 不还原它
                pair => System.Uri.UnescapeDataString(pair[0].Replace('+', ' ')),
                pair => System.Uri.UnescapeDataString((pair.Length > 1 ? pair[1] : string.Empty).Replace('+', ' ')));
    }

    sealed class RecordingHttpHandler : HttpMessageHandler
    {
        public string? LastUrl;
        public string? LastFormRaw;
        public Func<HttpResponseMessage>? Responder;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastUrl = request.RequestUri?.ToString();
            LastFormRaw = request.Content?.ReadAsStringAsync().GetAwaiter().GetResult();
            return Task.FromResult(Responder?.Invoke()
                ?? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}", Encoding.UTF8, "application/json") });
        }
    }
}
