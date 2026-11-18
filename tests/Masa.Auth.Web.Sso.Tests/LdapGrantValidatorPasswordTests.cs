// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Reflection;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Masa.Auth.Web.Sso.Infrastructure.Ldap;
using Masa.Auth.Web.Sso.Infrastructure.Validations;
using Masa.BuildingBlocks.StackSdks.Auth;
using Masa.BuildingBlocks.StackSdks.Auth.Service;
using Microsoft.Extensions.Logging.Abstractions;
using Masa.Utils.Ldap.Novell;
using Masa.Utils.Ldap.Novell.Entries;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary>
/// 空/缺密码不得签发 token，且不得触达 AD bind（依赖 bind 计数而非 AD 侧拒绝）。
/// </summary>
public class LdapGrantValidatorPasswordTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task Missing_or_blank_password_never_issues_a_token(string? password)
    {
        var harness = new Harness();

        var context = harness.CreateContext(password);
        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Equal(IdentityModel.OidcConstants.TokenErrors.InvalidGrant, context.Result.Error);
        Assert.Equal(0, harness.LdapProvider.UserAuthenticateCalls);
    }

    [Fact]
    public async Task Missing_password_parameter_altogether_never_issues_a_token()
    {
        var harness = new Harness();

        var context = harness.CreateContext(password: null, includePasswordField: false);
        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Equal(0, harness.LdapProvider.UserAuthenticateCalls);
    }

    [Fact]
    public async Task Blank_password_records_a_throttle_failure()
    {
        var harness = new Harness();

        var context = harness.CreateContext("   ");
        await harness.Validator.ValidateAsync(context);

        Assert.True(harness.Throttle.FailureCount > 0);
    }

    [Fact]
    public async Task Missing_user_name_is_rejected_before_any_ldap_work()
    {
        var harness = new Harness();

        var context = harness.CreateContext("p@ssw0rd", userName: null);
        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Equal(0, harness.LdapProvider.UserAuthenticateCalls);
    }

    sealed class Harness
    {
        public Harness()
        {
            Validator = new LdapGrantValidator(
                StubAuthClient(), // 不会走到：所有用例都在签发前失败
                new StubThirdPartyIdpService(),
                new StubLdapFactory(LdapProvider),
                Throttle,
                NullLogger<LdapGrantValidator>.Instance);
        }

        public StubLdapProvider LdapProvider { get; } = new();

        public StubThrottle Throttle { get; } = new();

        public LdapGrantValidator Validator { get; }

        public ExtensionGrantValidationContext CreateContext(
            string? password,
            string? userName = "mayue",
            bool includePasswordField = true)
        {
            var raw = new System.Collections.Specialized.NameValueCollection();
            if (userName is not null)
            {
                raw["userName"] = userName;
            }
            if (includePasswordField)
            {
                raw["password"] = password;
            }

            return new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Client = new Client { ClientId = "test-client" },
                    Raw = raw
                }
            };
        }

        static IAuthClient StubAuthClient()
        {
            var proxy = DispatchProxy.Create<IAuthClient, ThrowingProxy>();
            ((ThrowingProxy)(object)proxy).MemberName = nameof(IAuthClient.UserService);
            return proxy;
        }    }

    /// <summary>任何调用都视为测试失败：这些用例根本不该走到用户体系。</summary>
    class ThrowingProxy : DispatchProxy
    {
        public string MemberName { get; set; } = "";

        protected override object? Invoke(System.Reflection.MethodInfo? targetMethod, object?[]? args)
            => throw new Xunit.Sdk.XunitException(
                $"Unexpected call to {MemberName}.{targetMethod?.Name} — the validator should have rejected before provisioning.");
    }

    sealed class StubThrottle : LdapLoginThrottle
    {
        public int FailureCount { get; private set; }

        public StubThrottle() : base(null!, NullLogger<LdapLoginThrottle>.Instance) { }

        public override Task EnsureNotFrozenAsync(string? account) => Task.CompletedTask;

        public override Task RecordFailureAsync(string? account)
        {
            FailureCount++;
            return Task.CompletedTask;
        }

        public override Task ResetAsync(string? account) => Task.CompletedTask;
    }

    sealed class StubLdapFactory : ILdapFactory
    {
        readonly ILdapProvider _provider;

        public StubLdapFactory(ILdapProvider provider) => _provider = provider;

        public ILdapProvider CreateProvider(LdapOptions options) => _provider;
    }

    sealed class StubLdapProvider : ILdapProvider
    {
        public int UserAuthenticateCalls { get; private set; }

        public Task<bool> AuthenticateAsync(string userName, string password)
        {
            UserAuthenticateCalls++;
            return Task.FromResult(true);
        }

        public Task<LdapUser?> GetUserByUserNameAsync(string userName) => Task.FromResult<LdapUser?>(null);

        public Task<Novell.Directory.Ldap.LdapEntry?> GetGroupAsync(string groupName) => throw new NotSupportedException();
        public IAsyncEnumerable<LdapUser> GetUsersInGroupAsync(string groupName) => throw new NotSupportedException();
        public Task<LdapUser?> GetUsersByEmailAddressAsync(string emailAddress) => throw new NotSupportedException();
        public IAsyncEnumerable<LdapUser> GetAllUserAsync() => throw new NotSupportedException();
        public Task<List<Novell.Directory.Ldap.LdapEntry>> GetPagingUserAsync(int size) => throw new NotSupportedException();
        public Task AddUserAsync(LdapUser user, string password) => throw new NotSupportedException();
        public Task DeleteUserAsync(string distinguishedName) => throw new NotSupportedException();
    }

    sealed class StubThirdPartyIdpService : IThirdPartyIdpService
    {
        public Task<Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model.LdapOptionsModel?> GetLdapOptionsAsync(string scheme)
            => Task.FromResult<Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model.LdapOptionsModel?>(
                new Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model.LdapOptionsModel(
                    serverAddress: "ldap.example.com",
                    serverPort: 389,
                    baseDn: "DC=example,DC=com",
                    userSearchBaseDn: "DC=example,DC=com",
                    groupSearchBaseDn: "DC=example,DC=com",
                    rootUserDn: "CN=svc,DC=example,DC=com",
                    rootUserPassword: "secret"));

        public Task<List<Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model.ThirdPartyIdpModel>> GetAllAsync() => throw new NotSupportedException();
        public Task<List<Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model.ThirdPartyIdpModel>> GetAllFromCacheAsync() => throw new NotSupportedException();
        public Task<List<Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model.ThirdPartyIdpSelectModel>> GetSelectAsync(string? search, bool includeLdap) => throw new NotSupportedException();
    }
}
