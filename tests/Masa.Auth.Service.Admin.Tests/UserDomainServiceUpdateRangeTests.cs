// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Domain.Subjects.Aggregates;
using Masa.Auth.EntityFrameworkCore;
using Masa.Auth.EntityFrameworkCore.Repositories;
using Masa.Auth.Service.Admin.Domain.Permissions.Services;
using Masa.BuildingBlocks.Caching;
using Masa.BuildingBlocks.Data.UoW;
using Masa.BuildingBlocks.Ddd.Domain.Events;
using Masa.BuildingBlocks.SearchEngine.AutoComplete;
using Masa.BuildingBlocks.SearchEngine.AutoComplete.Response;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// UserDomainService.UpdateRangeAsync：真实的 UserDomainService + UserRepository + SQLite（PG 形态的过滤唯一索引），
/// 缓存/搜索引擎/事件总线用桩。LDAP 同步每次都会 new() 替换 User.Address，用例按这个真实形态构造。
/// </summary>
public class UserDomainServiceUpdateRangeTests : IDisposable
{
    readonly SqliteAuthDb _db = new();
    AuthDbContext Context => _db.Context;

    public void Dispose() => _db.Dispose();

    UserDomainService NewService()
    {
        //工作单元桩：SaveChangesAsync 直接保存上下文（真实实现也只是保存，提交由事件管线负责）
        var unitOfWork = StubProxy.Create<IUnitOfWork>((method, _) =>
            method.Name == nameof(IUnitOfWork.SaveChangesAsync) ? (true, Context.SaveChangesAsync()) : (false, null));
        var autoComplete = StubProxy.Create<IAutoCompleteClient>((method, _) =>
            method.Name == nameof(IAutoCompleteClient.SetBySpecifyDocumentAsync) ? (true, Task.FromResult(new SetResponse(true, ""))) : (false, null));

        return new UserDomainService(
            new UserRepository(Context, unitOfWork),
            autoComplete,
            new ListLogger(),
            null!,
            StubProxy.Create<IDistributedCacheClient>(),
            new RoleDomainService(StubProxy.Create<IDomainEventBus>()),
            Context,
            unitOfWork,
            null!,
            null!);
    }

    async Task<User> AddUserAsync(string account, string email, string phoneNumber, string? staffJobNumber = null)
    {
        var user = staffJobNumber == null
            ? new User(account, account, null, account, "", "", email, phoneNumber)
            : new User(account, account, null, account, "", "", email, phoneNumber,
                new Staff(account, account + "-staff", "", "", "", GenderTypes.Male, phoneNumber, email, staffJobNumber, null, StaffTypes.Internal, true));
        Context.Set<User>().Add(user);
        await Context.SaveChangesAsync();
        return user;
    }

    //AsNoTracking 绕过身份映射，读到的是数据库里的真实值
    Task<User> LoadAsync(Guid id) =>
        Context.Set<User>().AsNoTracking().Include(u => u.Staff).SingleAsync(u => u.Id == id);

    static void SimulateLdapUpdate(User user, string displayName)
        => user.UpdateBasicInfo(user.Name, displayName, GenderTypes.Male, "", "", "", "", new());

    //Filter 能识别的冲突：User.Email 与库内另一用户（owner，不在批次内）相同 -> 该用户被跳过，其余用户仍更新成功
    [Fact]
    public async Task UpdateRange_UserEmailConflictsWithDbUser_ConflictingUserSkipped_OthersUpdated()
    {
        var owner = await AddUserAsync("owner", "owner@x.com", "13800000021");
        var a = await AddUserAsync("a", "a@x.com", "13800000022");
        var c = await AddUserAsync("c", "c@x.com", "13800000023");
        var skipped = await AddUserAsync("skipped", "skipped@x.com", "13800000024", staffJobNumber: "J-SKIPPED");

        SimulateLdapUpdate(a, "a-updated");
        SimulateLdapUpdate(c, "c-updated");
        SimulateLdapUpdate(skipped, "skipped");
        Context.Entry(skipped).Property(nameof(User.Email)).CurrentValue = owner.Email;
        skipped.Staff!.UpdateBasicInfo("skipped", "skipped-staff-updated", GenderTypes.Male, "13800000024", "skipped@x.com");

        var batch = new List<User> { a, skipped, c };
        await NewService().UpdateRangeAsync(batch);

        Assert.Equal(new[] { a.Id, c.Id }, batch.Select(u => u.Id).ToArray());
        Assert.Equal("a-updated", (await LoadAsync(a.Id)).DisplayName);
        Assert.Equal("c-updated", (await LoadAsync(c.Id)).DisplayName);

        //被跳过用户（含 Staff）的修改未落库
        var skippedInDb = await LoadAsync(skipped.Id);
        Assert.Equal("skipped@x.com", skippedInDb.Email);
        Assert.Equal("skipped-staff", skippedInDb.Staff!.DisplayName);

        //被跳过用户的待提交修改已丢弃：同一工作单元里之后再 SaveChanges（如事务提交）不会再因它失败
        Assert.False(Context.ChangeTracker.HasChanges());
        await Context.SaveChangesAsync();
    }

    //全部被 Filter 跳过时也必须丢弃它们的待提交修改（不能因 users.Count == 0 提前返回就漏掉）
    [Fact]
    public async Task UpdateRange_AllUsersSkipped_StillDiscardsTheirPendingChanges()
    {
        var owner = await AddUserAsync("owner", "owner@x.com", "13800000021");
        var skipped = await AddUserAsync("skipped", "skipped@x.com", "13800000024", staffJobNumber: "J-SKIPPED");
        SimulateLdapUpdate(skipped, "skipped");
        Context.Entry(skipped).Property(nameof(User.Email)).CurrentValue = owner.Email;

        var batch = new List<User> { skipped };
        await NewService().UpdateRangeAsync(batch);

        Assert.Empty(batch);
        Assert.False(Context.ChangeTracker.HasChanges());
        await Context.SaveChangesAsync();
    }

    //Filter 识别不了的冲突（Staff.JobNumber 唯一索引）：批量保存失败 -> 逐条兜底，只跳过冲突的那个
    [Fact]
    public async Task UpdateRange_ConflictNotSeenByFilter_OneByOneFallbackKeepsOthers()
    {
        await AddUserAsync("owner", "owner@x.com", "13800000021", staffJobNumber: "J1");
        var a = await AddUserAsync("a", "a@x.com", "13800000022");
        var c = await AddUserAsync("c", "c@x.com", "13800000023");
        var x = await AddUserAsync("x", "x@x.com", "13800000024", staffJobNumber: "J2");

        SimulateLdapUpdate(a, "a-updated");
        SimulateLdapUpdate(c, "c-updated");
        x.Staff!.UpdateJobNumber("J1"); //与 owner 的 Staff 工号冲突：Filter 只比较 User 字段

        await NewService().UpdateRangeAsync(new List<User> { a, x, c });

        Assert.Equal("a-updated", (await LoadAsync(a.Id)).DisplayName);
        Assert.Equal("c-updated", (await LoadAsync(c.Id)).DisplayName);
        Assert.Equal("J2", (await LoadAsync(x.Id)).Staff!.JobNumber);
    }

    sealed class ListLogger : Microsoft.Extensions.Logging.ILogger<UserDomainService>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => true;

        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId,
            TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }
    }
}
