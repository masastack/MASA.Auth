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
/// 用 SQLite 内存库验证恢复的过滤唯一索引与软删过滤的实际行为:
/// 这些约束丢失时不报错、只悄悄产生脏数据(如已软删用户在 LDAP 同步/登录路径"复活"),
/// 必须有测试兜底。
/// 大小写语义的分层:DB 层唯一索引为大小写敏感(挡完全相同的重复,即 LDAP 竞态 bug);
/// 大小写变体由应用层承担(UserUniqueKeyQueries 的 LOWER() 预检查),SS 由 CI 列排序规则在
/// DB 层直接拒绝。
/// </summary>
public class FilteredUniqueIndexTests : IDisposable
{
    readonly SqliteConnection _connection;
    readonly AuthDbContext _dbContext;

    public FilteredUniqueIndexTests()
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
    public async Task SameAccount_SecondActiveUser_IsRejected()
    {
        await AddUserAsync("zhangsan", "a@x.com", "13800000001");

        var ex = await Record.ExceptionAsync(() => AddUserAsync("zhangsan", "b@x.com", "13800000002"));

        Assert.IsType<DbUpdateException>(ex);
    }

    [Fact]
    public async Task SameAccount_AfterSoftDelete_IsAllowed()
    {
        var first = await AddUserAsync("zhangsan", "a@x.com", "13800000001");
        _dbContext.Entry(first).Property("IsDeleted").CurrentValue = true;
        await _dbContext.SaveChangesAsync();

        var ex = await Record.ExceptionAsync(() => AddUserAsync("zhangsan", "b@x.com", "13800000002"));

        Assert.Null(ex);
    }

    //大小写变体在 PG 的 DB 层唯一索引(大小写敏感)下允许——这是已接受的取舍:
    //大小写变体的唯一性由应用层承担(UserUniqueKeyQueries 的 LOWER() 预检查拦住顺序流程,
    //仅并发窗口可能漏过);SS 侧由 CI 列排序规则在 DB 层直接拒绝。本测试走共享模型
    //(即 PG 形态),固定住这个边界:将来若恢复 DB 层大小写不敏感约束,此处会失败并提醒同步更新。
    [Fact]
    public async Task SameAccount_DifferentCase_IsAllowedByDb()
    {
        await AddUserAsync("ZhangSan", "a@x.com", "13800000001");

        var ex = await Record.ExceptionAsync(() => AddUserAsync("zhangsan", "b@x.com", "13800000002"));

        Assert.Null(ex);
    }

    [Fact]
    public async Task SameEmail_DifferentCase_IsAllowedByDb()
    {
        await AddUserAsync("user_a", "Zhang@Corp.com", "13800000001");

        var ex = await Record.ExceptionAsync(() => AddUserAsync("user_b", "zhang@corp.com", "13800000002"));

        Assert.Null(ex);
    }

    [Fact]
    public async Task EmptyEmail_AreNotUnique()
    {
        var ex = await Record.ExceptionAsync(() => AddUserAsync("user_a", "", "13800000001"));

        Assert.Null(ex);

        ex = await Record.ExceptionAsync(() => AddUserAsync("user_b", "", "13800000002"));

        Assert.Null(ex);
    }

    [Fact]
    public async Task ThirdPartyUser_SameIdentityForSameIdp_IsRejected()
    {
        var idpId = await AddIdpAsync("github_test");

        await AddThirdPartyUserAsync(idpId, "object-guid-1", "tp_a");

        var ex = await Record.ExceptionAsync(() => AddThirdPartyUserAsync(idpId, "object-guid-1", "tp_b"));

        Assert.IsType<DbUpdateException>(ex);
    }

    [Fact]
    public async Task ThirdPartyUser_SameIdentityForDifferentIdp_IsAllowed()
    {
        var idp1 = await AddIdpAsync("wechat_test");
        var idp2 = await AddIdpAsync("dingding_test");

        await AddThirdPartyUserAsync(idp1, "object-guid-1", "tp_a");

        var ex = await Record.ExceptionAsync(() => AddThirdPartyUserAsync(idp2, "object-guid-1", "tp_b"));

        Assert.Null(ex);
    }

    //DetachPendingUserGraphs 的行为契约：失败批次的用户图（含 owned 类型）被整体 detach，
    //同上下文中其他用户的待提交变更（含 owned 替换）不受影响、仍可正常保存。
    [Fact]
    public async Task DetachPendingUserGraphs_DetachesOwnedGraph_AndKeepsOtherUsersPendingChanges()
    {
        //keeper：已入库用户，带一笔待提交的 owned（Address）替换——detach 后必须存活
        var keeper = await AddUserAsync("keeper", "keeper@x.com", "13800000011");
        keeper.UpdateBasicInfo("keeper", "keeper", Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum.GenderTypes.Male, "", "", "", "",
            new AddressValue("keeper-new-addr", "p", "c", "d"));

        //loser：未入库用户（Added），Account 与 keeper 冲突（若保存会被唯一索引拒绝）
        var loser = new User(default, "loser", "loser", null, "", "keeper", "", "", "", "",
            "13800000012", "", "loser@x.com", new AddressValue("loser-addr", "p", "c", "d"),
            Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum.GenderTypes.Male);
        _dbContext.Set<User>().Add(loser);

        _dbContext.DetachPendingUserGraphs(new[] { loser.Id });

        Assert.Equal(EntityState.Detached, _dbContext.Entry(loser).State);

        //owned 实例随 owner 一起被 detach（EF Core 级联）——若 EF 不级联，此断言失败，
        //需要恢复 DetachPendingUserGraphs 中的 owned 实例收集逻辑
        Assert.DoesNotContain(_dbContext.ChangeTracker.Entries(),
            e => e.Entity is AddressValue av && av.Address == "loser-addr");

        //其他用户的待提交变更（owned 替换）不受影响，仍可正常保存
        var keeperAddressEntry = _dbContext.ChangeTracker.Entries()
            .Single(e => e.Entity is AddressValue av && av.Address == "keeper-new-addr");
        Assert.NotEqual(EntityState.Detached, keeperAddressEntry.State);

        var saveEx = await Record.ExceptionAsync(() => _dbContext.SaveChangesAsync());
        Assert.Null(saveEx);
    }

    //UserDomainService.UpdateRangeAsync 路径的契约：Filter 把冲突用户移出待保存列表后，这些用户仍是
    //已追踪且已修改的实体（LDAP 同步里是查库得到、已被改过的 User + Staff）。不丢弃它们，冲突修改会在
    //随后的每一次 SaveChanges（批量保存、逐条兜底、乃至事务提交）里被重新提交并失败。
    //以下用例固定该前置事实及 DetachPendingUserGraphs 的修复效果。
    //LDAP 同步每次都用 new AddressValue() 替换 owned 的 Address 实例，所以夹具里用户的 Address 都是被替换过的：
    //被替换的旧实例会以 Deleted 状态残留，EF 不会随 owner 级联 detach 它。
    [Fact]
    public async Task UpdateBatch_SkippedConflictingUser_IfStillTracked_FailsTheWholeSave()
    {
        await ArrangeUpdateBatchWithConflictingUserAsync();

        var ex = await Record.ExceptionAsync(() => _dbContext.SaveChangesAsync());

        Assert.IsType<DbUpdateException>(ex);
    }

    [Fact]
    public async Task UpdateBatch_SkippedConflictingUser_Detached_OtherUsersStillUpdated()
    {
        var batch = await ArrangeUpdateBatchWithConflictingUserAsync();

        _dbContext.DetachPendingUserGraphs(new[] { batch.Skipped.Id });

        Assert.Equal(EntityState.Detached, _dbContext.Entry(batch.Skipped).State);
        Assert.Equal(EntityState.Detached, _dbContext.Entry(batch.Skipped.Staff!).State);

        //被替换掉的旧 Address 实例（Deleted 孤儿）也必须一并 detach，否则它会把该行的 owned 列置 NULL 并让后续每次保存失败
        Assert.DoesNotContain(_dbContext.ChangeTracker.Entries(),
            e => e.State == EntityState.Deleted && e.Metadata.FindOwnership() is { } ownership
                && ownership.PrincipalEntityType.ClrType == typeof(User)
                && Equals(e.Property(ownership.Properties[0].Name).CurrentValue, batch.Skipped.Id));

        var ex = await Record.ExceptionAsync(() => _dbContext.SaveChangesAsync());
        Assert.Null(ex);

        //批次内其余用户的更新已落库
        Assert.Equal("a-updated", (await LoadUserAsync(batch.OtherA.Id)).DisplayName);
        Assert.Equal("c-updated", (await LoadUserAsync(batch.OtherC.Id)).DisplayName);

        //被跳过的用户（含 Staff）的待提交修改被整体丢弃：库里仍是原值
        var skipped = await LoadUserAsync(batch.Skipped.Id);
        Assert.Equal("skipped@x.com", skipped.Email);
        Assert.Equal("skipped-staff", skipped.Staff!.DisplayName);
    }

    async Task<(User Owner, User Skipped, User OtherA, User OtherC)> ArrangeUpdateBatchWithConflictingUserAsync()
    {
        //owner：库内另一用户，不在更新批次内
        var owner = await AddUserAsync("owner", "owner@x.com", "13800000021");
        var otherA = await AddUserAsync("other_a", "a@x.com", "13800000022");
        var otherC = await AddUserAsync("other_c", "c@x.com", "13800000023");
        var skipped = new User("skipped", "skipped", null, "skipped", "", "", "skipped@x.com", "13800000024",
            new Staff("skipped", "skipped-staff", "", "", "", GenderTypes.Male, "13800000024", "skipped@x.com", "", null, StaffTypes.Internal, true));
        _dbContext.Set<User>().Add(skipped);
        await _dbContext.SaveChangesAsync();

        //更新批次：otherA/otherC 正常修改；skipped 的邮箱改成 owner 的邮箱（命中 Email 唯一索引），Staff 同时有修改
        otherA.UpdateBasicInfo("other_a", "a-updated", GenderTypes.Male, "", "", "", "", new());
        otherC.UpdateBasicInfo("other_c", "c-updated", GenderTypes.Male, "", "", "", "", new());
        _dbContext.Entry(skipped).Property(nameof(User.Email)).CurrentValue = owner.Email;
        skipped.UpdateBasicInfo("skipped", "skipped", GenderTypes.Male, "", "", "", "", new());
        skipped.Staff!.UpdateBasicInfo("skipped", "skipped-staff-updated", GenderTypes.Male, "13800000024", "skipped@x.com");

        return (owner, skipped, otherA, otherC);
    }

    //UserDomainService.UpdateRangeOneByOneAsync 的步骤：批量保存失败后，先 detach 整批、再逐条 Update + SaveChanges，
    //冲突的那条被跳过并 detach，其余用户仍应更新成功（含被替换了 Address 的用户）。
    [Fact]
    public async Task UpdateBatch_OneByOneFallback_OnlyConflictingUserIsSkipped()
    {
        await AddUserWithStaffAsync("owner", "owner@x.com", "13800000021", jobNumber: "J1");
        var a = await AddUserAsync("a", "a@x.com", "13800000022");
        var c = await AddUserAsync("c", "c@x.com", "13800000023");
        var x = await AddUserWithStaffAsync("x", "x@x.com", "13800000024", jobNumber: "J2");

        a.UpdateBasicInfo("a", "a-updated", GenderTypes.Male, "", "", "", "", new());
        c.UpdateBasicInfo("c", "c-updated", GenderTypes.Male, "", "", "", "", new());
        x.Staff!.UpdateJobNumber("J1"); //与 owner 的工号冲突（Staff.JobNumber 过滤唯一索引）

        var batch = new List<User> { a, x, c };
        _dbContext.Set<User>().UpdateRange(batch);
        var batchEx = await Record.ExceptionAsync(() => _dbContext.SaveChangesAsync());
        Assert.IsType<DbUpdateException>(batchEx);

        _dbContext.DetachPendingUserGraphs(batch.Select(u => u.Id));
        foreach (var user in batch)
        {
            try
            {
                _dbContext.Set<User>().Update(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                _dbContext.DetachPendingUserGraphs(new[] { user.Id });
            }
        }

        Assert.Equal("a-updated", (await LoadUserAsync(a.Id)).DisplayName);
        Assert.Equal("c-updated", (await LoadUserAsync(c.Id)).DisplayName);
        Assert.Equal("J2", (await LoadUserAsync(x.Id)).Staff!.JobNumber);
    }

    async Task<User> AddUserWithStaffAsync(string account, string email, string phoneNumber, string jobNumber)
    {
        var user = new User(account, account, null, account, "", "", email, phoneNumber,
            new Staff(account, account + "-staff", "", "", "", GenderTypes.Male, phoneNumber, email, jobNumber, null, StaffTypes.Internal, true));
        _dbContext.Set<User>().Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    //AsNoTracking 绕过身份映射，读到的是数据库里的真实值
    Task<User> LoadUserAsync(Guid id) =>
        _dbContext.Set<User>().AsNoTracking().Include(u => u.Staff).SingleAsync(u => u.Id == id);

    async Task<Guid> AddIdpAsync(string name)
    {
        var idp = new Masa.Auth.Domain.Subjects.Aggregates.ThirdPartyIdp(
            name, name, "", true, Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum.ThirdPartyIdpTypes.Customize,
            "client-id", "client-secret", "/callback", "https://idp.example/authorize", "https://idp.example/token", "https://idp.example/userinfo", Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum.AuthenticationTypes.OAuth, false, null);
        _dbContext.Set<Masa.Auth.Domain.Subjects.Aggregates.ThirdPartyIdp>().Add(idp);
        await _dbContext.SaveChangesAsync();
        return idp.Id;
    }

    async Task<Masa.Auth.Domain.Subjects.Aggregates.User> AddUserAsync(string account, string email, string phoneNumber)
    {
        var user = new Masa.Auth.Domain.Subjects.Aggregates.User(
            account, account, null, account, "", "", email, phoneNumber);
        _dbContext.Set<Masa.Auth.Domain.Subjects.Aggregates.User>().Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    async Task AddThirdPartyUserAsync(Guid thirdPartyIdpId, string identity, string suffix)
    {
        var account = $"tp_{suffix}";
        var user = new Masa.Auth.Domain.Subjects.Aggregates.User(
            account, account, null, account, "", "", $"{account}@x.com", "",
            new Masa.Auth.Domain.Subjects.Aggregates.ThirdPartyUser(thirdPartyIdpId, identity, "{}"));
        _dbContext.Set<Masa.Auth.Domain.Subjects.Aggregates.User>().Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }
}
