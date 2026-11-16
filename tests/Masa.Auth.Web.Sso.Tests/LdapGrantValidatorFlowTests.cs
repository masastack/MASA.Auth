// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Reflection;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Masa.Auth.Web.Sso.Infrastructure.Ldap;
using Masa.Auth.Web.Sso.Infrastructure.Validations;
using Masa.BuildingBlocks.StackSdks.Auth;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
using Masa.BuildingBlocks.StackSdks.Auth.Service;
using Masa.BuildingBlocks.StackSdks.Config.Consts;
using Masa.Utils.Ldap.Novell;
using Masa.Utils.Ldap.Novell.Entries;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Masa.Auth.Web.Sso.Tests;

/// <summary><c>grant_type=ldap</c> 放行/拒绝路径（凭据、限流、枚举防护、元字符等）。</summary>
public class LdapGrantValidatorFlowTests
{
    static readonly Guid UserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    [Fact]
    public async Task Correct_credentials_for_an_enabled_user_issue_a_token_and_reset_the_throttle()
    {
        var harness = new Harness();
        var context = harness.CreateContext("p@ssw0rd");

        await harness.Validator.ValidateAsync(context);

        Assert.False(context.Result.IsError, context.Result.ErrorDescription);
        Assert.Equal(UserId.ToString(), context.Result.Subject!.FindFirst("sub")!.Value);
        Assert.Equal("mayue", context.Result.Subject.FindFirst(IdentityClaimConsts.DOMAIN_NAME)!.Value);
        Assert.Equal(1, harness.Throttle.ResetCount);
        Assert.Equal(0, harness.Throttle.FailureCount);
        Assert.Equal("example\\mayue", harness.LdapProvider.LastUserBindName);
    }

    [Fact]
    public async Task Wrong_password_is_rejected_and_counts_towards_the_throttle()
    {
        var harness = new Harness { UserBindSucceeds = false };
        var context = harness.CreateContext("wrong");

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Equal(1, harness.Throttle.FailureCount);
        Assert.Equal(0, harness.Throttle.ResetCount);
        Assert.Equal(0, harness.AuthClient.ThirdPartyLookups);
    }

    [Fact]
    public async Task Account_disabled_in_AD_is_rejected_with_the_same_error_as_a_wrong_password()
    {
        var wrong = new Harness { UserBindSucceeds = false };
        var wrongContext = wrong.CreateContext("wrong");
        await wrong.Validator.ValidateAsync(wrongContext);

        var disabled = new Harness { AdDisabled = true };
        var disabledContext = disabled.CreateContext("p@ssw0rd");
        await disabled.Validator.ValidateAsync(disabledContext);

        Assert.True(disabledContext.Result.IsError);
        Assert.Equal(1, disabled.Throttle.FailureCount);
        Assert.Equal(wrongContext.Result.ErrorDescription, disabledContext.Result.ErrorDescription);
    }

    [Fact]
    public async Task User_disabled_in_MASA_cannot_get_a_token_even_with_the_right_AD_password()
    {
        var harness = new Harness { MasaEnabled = false };
        var context = harness.CreateContext("p@ssw0rd");

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Null(context.Result.Subject);
        Assert.Equal(1, harness.Throttle.FailureCount);
        Assert.Equal(0, harness.Throttle.ResetCount);
    }

    [Fact]
    public async Task User_deleted_in_MASA_cannot_get_a_token()
    {
        var harness = new Harness { MasaDeleted = true };
        var context = harness.CreateContext("p@ssw0rd");

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Null(context.Result.Subject);
    }

    [Fact]
    public async Task Infrastructure_failure_does_not_leak_internal_details_nor_count_as_a_credential_failure()
    {
        var harness = new Harness { LdapException = new InvalidOperationException("connect to ldap-internal.corp:389 failed") };
        var context = harness.CreateContext("p@ssw0rd");

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.DoesNotContain("ldap-internal", context.Result.ErrorDescription);
        Assert.Equal(0, harness.Throttle.FailureCount);
    }

    [Theory]
    [InlineData("*")]
    [InlineData("a*")]
    [InlineData("(uid=*)")]
    [InlineData("mayue)(objectClass=*")]
    [InlineData("mayue\\00")]
    public async Task Ldap_metacharacters_in_the_user_name_are_rejected_before_any_bind(string userName)
    {
        var harness = new Harness();
        var context = harness.CreateContext("p@ssw0rd", userName);

        await harness.Validator.ValidateAsync(context);

        Assert.True(context.Result.IsError);
        Assert.Equal(0, harness.LdapProvider.AuthenticateCalls);
        Assert.Equal(0, harness.Throttle.FailureCount);
    }

    [Theory]
    [InlineData("DC=example,DC=com", "example")]
    [InlineData("DC=corp,DC=example,DC=com", "corp")] // 旧的贪婪正则在这里会得到 "corp,DC=example"
    [InlineData("OU=Users,DC=corp,DC=example,DC=com", "corp")]
    [InlineData("ou=users, dc=corp ,dc=com", "corp")]
    [InlineData("DC=corp", "corp")]
    [InlineData("OU=Users,O=Corp", null)]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void Windows_domain_is_the_first_DC_component_of_the_base_dn(string? baseDn, string? expected)
    {
        Assert.Equal(expected, LdapGrantValidator.GetNetBiosDomain(baseDn));
    }

    sealed class Harness
    {
        public bool UserBindSucceeds { get; set; } = true;
        public bool AdDisabled { get; set; }
        public bool MasaEnabled { get; set; } = true;
        public bool MasaDeleted { get; set; }
        public Exception? LdapException { get; set; }

        public StubLdapProvider LdapProvider { get; }
        public StubThrottle Throttle { get; } = new();
        public AuthClientProxy AuthClient { get; }
        public LdapGrantValidator Validator { get; }

        public Harness()
        {
            LdapProvider = new StubLdapProvider(this);
            var authClient = DispatchProxy.Create<IAuthClient, AuthClientProxy>();
            AuthClient = (AuthClientProxy)(object)authClient;
            AuthClient.Owner = this;

            Validator = new LdapGrantValidator(
                authClient,
                new StubThirdPartyIdpService(),
                new StubLdapFactory(LdapProvider),
                Throttle,
                NullLogger<LdapGrantValidator>.Instance);
        }

        public ExtensionGrantValidationContext CreateContext(string? password, string? userName = "mayue")
        {
            var raw = new System.Collections.Specialized.NameValueCollection { ["userName"] = userName, ["password"] = password };

            return new ExtensionGrantValidationContext
            {
                Request = new ValidatedTokenRequest
                {
                    Client = new Client { ClientId = "test-client" },
                    Raw = raw
                }
            };
        }

        public LdapUser BuildLdapUser() => new()
        {
            SamAccountName = "mayue",
            Name = "mayue",
            DisplayName = "Ma Yue",
            ObjectGuid = "ad-guid-1",
            UserAccountControl = AdDisabled
                ? UserAccountControl.NormalAccount | UserAccountControl.AccountDisabled
                : UserAccountControl.NormalAccount
        };

        public UserModel BuildMasaUser() => new()
        {
            Id = UserId,
            Account = "mayue",
            DisplayName = "Ma Yue",
            Enabled = MasaEnabled,
            IsDeleted = MasaDeleted
        };
    }

    class AuthClientProxy : DispatchProxy
    {
        public Harness Owner { get; set; } = null!;

        public int ThirdPartyLookups { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            switch (targetMethod?.Name)
            {
                case "get_UserService":
                    var userService = DispatchProxy.Create<IUserService, UserServiceProxy>();
                    ((UserServiceProxy)(object)userService).Client = this;
                    return userService;

                case "get_OperationLogService":
                    return DispatchProxy.Create<IOperationLogService, OperationLogServiceProxy>();
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
                    return Task.FromResult<UserModel?>(Client.Owner.BuildMasaUser());
            }

            throw new Xunit.Sdk.XunitException($"Unexpected call to IUserService.{targetMethod?.Name}");
        }
    }

    class OperationLogServiceProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) => Task.CompletedTask;
    }

    sealed class StubThrottle : LdapLoginThrottle
    {
        public int FailureCount { get; private set; }
        public int ResetCount { get; private set; }

        public StubThrottle() : base(null!, NullLogger<LdapLoginThrottle>.Instance) { }

        public override Task EnsureNotFrozenAsync(string? account) => Task.CompletedTask;

        public override Task RecordFailureAsync(string? account)
        {
            FailureCount++;
            return Task.CompletedTask;
        }

        public override Task ResetAsync(string? account)
        {
            ResetCount++;
            return Task.CompletedTask;
        }
    }

    sealed class StubLdapFactory : ILdapFactory
    {
        readonly ILdapProvider _provider;

        public StubLdapFactory(ILdapProvider provider) => _provider = provider;

        public ILdapProvider CreateProvider(LdapOptions options) => _provider;
    }

    sealed class StubLdapProvider : ILdapProvider
    {
        readonly Harness _owner;

        public StubLdapProvider(Harness owner) => _owner = owner;

        public int AuthenticateCalls { get; private set; }
        public string? LastUserBindName { get; private set; }

        public Task<bool> AuthenticateAsync(string userName, string password)
        {
            AuthenticateCalls++;
            if (_owner.LdapException is not null)
            {
                throw _owner.LdapException;
            }

            if (AuthenticateCalls == 1)
            {
                return Task.FromResult(true);
            }

            LastUserBindName = userName;
            return Task.FromResult(_owner.UserBindSucceeds);
        }

        public Task<LdapUser?> GetUserByUserNameAsync(string userName) => Task.FromResult<LdapUser?>(_owner.BuildLdapUser());

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
        public Task<LdapOptionsModel?> GetLdapOptionsAsync(string scheme)
            => Task.FromResult<LdapOptionsModel?>(
                new LdapOptionsModel(
                    serverAddress: "ldap.example.com",
                    serverPort: 389,
                    baseDn: "DC=example,DC=com",
                    userSearchBaseDn: "DC=example,DC=com",
                    groupSearchBaseDn: "DC=example,DC=com",
                    rootUserDn: "CN=svc,DC=example,DC=com",
                    rootUserPassword: "secret"));

        public Task<List<ThirdPartyIdpModel>> GetAllAsync() => throw new NotSupportedException();
        public Task<List<ThirdPartyIdpModel>> GetAllFromCacheAsync() => throw new NotSupportedException();
        public Task<List<ThirdPartyIdpSelectModel>> GetSelectAsync(string? search, bool includeLdap) => throw new NotSupportedException();
    }
}
