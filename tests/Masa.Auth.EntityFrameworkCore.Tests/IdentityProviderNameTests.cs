// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Domain.Subjects.Aggregates;
using Masa.Auth.EntityFrameworkCore;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Masa.Auth.EntityFrameworkCore.Tests;

/// <summary>
/// IdentityProvider.Name(scheme)语义的契约测试:
/// 1. 查询口径为应用层不区分大小写(IdentityProviderQueries.NameEquals → LOWER(列) = lower(参数)),
///    与 SSO 侧 RemoteAuthenticationDefaultsProvider 的 OrdinalIgnoreCase 对齐——若有人改回
///    精确匹配(Name == x),大小写变体的 scheme 会查空并抛 IDENTITY_PROVIDER_NOT_FOUND,此处会失败。
/// 2. Name 恢复为过滤唯一索引(NOT IsDeleted)后:活跃同名被 DB 拒绝;软删后可重建。
///    大小写变体仍允许共存(DB 层大小写敏感),其唯一性由应用层查重(AddThirdPartyIdpAsync 走
///    NameEquals)承担——与 User.Account/Email 的既定分层取舍一致。
/// </summary>
public class IdentityProviderNameTests : IDisposable
{
    readonly SqliteConnection _connection;
    readonly AuthDbContext _dbContext;

    public IdentityProviderNameTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        //注册与 PG 部署一致的实体配置(与 PG 设计期工厂相同:auth 配置 + OpenIdConnect 配置)
        AuthDbContext.RegisterAssembly(typeof(Masa.Auth.EntityFrameworkCore.AuthPostgreSqlDbContextFactory).Assembly);
        AuthDbContext.RegisterAssembly(typeof(Masa.Contrib.Authentication.OpenIdConnect.EFCore.PostgreSql.OpenIdConnectEFPostgreSql).Assembly);

        var optionsBuilder = new Microsoft.EntityFrameworkCore.MasaDbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.DbContextOptionsBuilder.UseSqlite(_connection);
        _dbContext = new AuthDbContext(optionsBuilder.MasaOptions);

        //EnsureCreated 生成的 DDL 中,DeviceFlowCodes 的唯一索引过滤器引用了 IsDeleted——该列只在
        //运行时启用软删选项(UseFilter)后才以影子属性存在,设计期模型里没有,这是模型的既有遗留问题
        //(生产走迁移不受影响)。测试里剔除该语句后执行其余 DDL 建表。
        var script = _dbContext.Database.GenerateCreateScript();
        var statements = script.Split(new[] { ";\r\n", ";\n" }, StringSplitOptions.RemoveEmptyEntries);
        var executable = statements
            .Where(s => !(s.Contains("CREATE", StringComparison.OrdinalIgnoreCase)
                && s.Contains("INDEX", StringComparison.OrdinalIgnoreCase)
                && s.Contains("DeviceFlowCodes", StringComparison.OrdinalIgnoreCase)
                && s.Contains("IsDeleted", StringComparison.OrdinalIgnoreCase)));
        foreach (var statement in executable)
        {
            _dbContext.Database.ExecuteSqlRaw(statement.Trim());
        }
    }

    [Fact]
    public async Task NameEquals_CaseVariants_Hit()
    {
        await AddIdpAsync("GitHub");

        var byLower = await _dbContext.Set<IdentityProvider>().AsNoTracking()
            .FirstOrDefaultAsync(IdentityProviderQueries.NameEquals<IdentityProvider>("github"));
        var byUpper = await _dbContext.Set<IdentityProvider>().AsNoTracking()
            .FirstOrDefaultAsync(IdentityProviderQueries.NameEquals<IdentityProvider>("GITHUB"));
        var byMiss = await _dbContext.Set<IdentityProvider>().AsNoTracking()
            .FirstOrDefaultAsync(IdentityProviderQueries.NameEquals<IdentityProvider>("git-hub"));

        Assert.Equal("GitHub", byLower?.Name);
        Assert.Equal("GitHub", byUpper?.Name);
        Assert.Null(byMiss);
    }

    //大小写变体在 DB 层唯一索引(大小写敏感)下允许——与 User.Account 同源的已接受取舍:
    //变体的唯一性由应用层查重承担。本测试固定住这个边界:将来若恢复 DB 层大小写不敏感约束
    //(如 LOWER(Name) 表达式索引或 CI 列),此处会失败并提醒同步更新分层注释。
    [Fact]
    public async Task SameName_DifferentCase_IsAllowedByDb()
    {
        await AddIdpAsync("GitHub");

        var ex = await Record.ExceptionAsync(() => AddIdpAsync("github"));

        Assert.Null(ex);
    }

    [Fact]
    public async Task SameName_SecondActiveIdp_IsRejected()
    {
        await AddIdpAsync("dup_name");

        var ex = await Record.ExceptionAsync(() => AddIdpAsync("dup_name"));

        Assert.IsType<DbUpdateException>(ex);
    }

    [Fact]
    public async Task SameName_AfterSoftDelete_IsAllowed()
    {
        var first = await AddIdpAsync("dup_name");
        _dbContext.Entry(first).Property("IsDeleted").CurrentValue = true;
        await _dbContext.SaveChangesAsync();

        var ex = await Record.ExceptionAsync(() => AddIdpAsync("dup_name"));

        Assert.Null(ex);
    }

    async Task<ThirdPartyIdp> AddIdpAsync(string name)
    {
        var idp = new ThirdPartyIdp(
            name, name, "", true, ThirdPartyIdpTypes.Customize,
            "client-id", "client-secret", "/callback",
            "https://idp.example/authorize", "https://idp.example/token", "https://idp.example/userinfo",
            AuthenticationTypes.OAuth, false, null);
        _dbContext.Set<ThirdPartyIdp>().Add(idp);
        await _dbContext.SaveChangesAsync();
        return idp;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
