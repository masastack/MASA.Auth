// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Data.Common;
using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Masa.Auth.Contracts.Admin.Subjects;
using Masa.Auth.Domain.Permissions.Aggregates;
using Masa.Auth.Service.Admin.Application.Permissions;
using Masa.Auth.Service.Admin.Application.Permissions.Queries;
using Masa.Auth.Service.Admin.Infrastructure.CacheModels;
using Masa.BuildingBlocks.Caching;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// QueryHandler.GetPermissionsByRoleAsync：缓存路径的子角色必须继续走缓存，只有缓存整体落空才回源 DB。
/// 缓存用桩实现并记录调用，DB 用 SQLite 内存库并统计查询次数。
/// </summary>
public class QueryHandlerRolePermissionTests : IDisposable
{
    readonly CountingInterceptor _dbQueries = new();
    readonly SqliteAuthDb _db;
    readonly Dictionary<string, CacheRole> _cacheStore = new();
    readonly IDistributedCacheClient _cacheClient;
    int _cacheGetListCalls;

    public QueryHandlerRolePermissionTests()
    {
        _db = new SqliteAuthDb(_dbQueries);
        //种子数据只关心角色关系，不建 Permission 行
        _db.Context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = OFF");

        _cacheClient = StubProxy.Create<IDistributedCacheClient>((method, args) =>
        {
            if (method.Name != "GetListAsync" || !method.IsGenericMethod)
                return (false, null);

            _cacheGetListCalls++;
            var keys = ((IEnumerable<string>)args![0]!).ToList();
            IEnumerable<CacheRole?> result = keys.Select(key => _cacheStore.TryGetValue(key, out var role) ? role : null).ToList();
            return (true, Task.FromResult(result));
        });
    }

    public void Dispose() => _db.Dispose();

    //GetPermissionsByRoleAsync 只用到 AuthDbContext 与缓存客户端
    QueryHandler NewHandler() => new(null!, null!, _db.Context, null!, _cacheClient, null!, null!, null!, null!);

    static Guid Id(int n) => new(n, 0, 0, new byte[8]);

    void Cache(int roleId, int[] children, params (int Permission, bool Effect)[] permissions)
        => _cacheStore[CacheKey.RoleKey(Id(roleId))] = new CacheRole
        {
            Name = "role",
            Enabled = true,
            ChildrenRoles = children.Select(Id).ToList(),
            Permissions = permissions.Select(p => new SubjectPermissionRelationDto(Id(p.Permission), p.Effect)).ToList()
        };

    static async Task<List<Guid>> QueryAsync(QueryHandler handler, params Guid[] roles)
    {
        var query = new PermissionsByRoleQuery(roles.ToList());
        await handler.GetPermissionsByRoleAsync(query);
        return query.Result;
    }

    static IEnumerable<Guid> Sorted(IEnumerable<Guid> ids) => ids.OrderBy(id => id);

    // ---------- 缓存路径 ----------

    [Fact]
    public async Task Cache_Children_AreResolvedFromCache_NotFromDb()
    {
        //DB 是空的：若子角色回源 DB，既会产生 DB 查询，也拿不到 2、3 号权限
        Cache(100, new[] { 101 }, (1, true));
        Cache(101, new[] { 102 }, (2, true));
        Cache(102, Array.Empty<int>(), (3, true));

        var result = await QueryAsync(NewHandler(), Id(100));

        Assert.Equal(Sorted(new[] { Id(1), Id(2), Id(3) }), Sorted(result));
        Assert.Equal(0, _dbQueries.Count);
    }

    [Fact]
    public async Task Cache_ParentReject_FiltersChildPermission()
    {
        Cache(100, new[] { 101 }, (1, true), (2, false));
        Cache(101, Array.Empty<int>(), (2, true), (3, true));

        var result = await QueryAsync(NewHandler(), Id(100));

        Assert.Equal(Sorted(new[] { Id(1), Id(3) }), Sorted(result));
        Assert.Equal(0, _dbQueries.Count);
    }

    [Fact]
    public async Task Cache_ChildNotCached_IsSkipped_AndDoesNotFallBackToDb()
    {
        //原有语义：子角色缓存未命中视为无权限，只有"整体落空"才回源 DB
        Cache(100, new[] { 101 }, (1, true));

        var result = await QueryAsync(NewHandler(), Id(100));

        Assert.Equal(new[] { Id(1) }, result);
        Assert.Equal(0, _dbQueries.Count);
    }

    [Fact]
    public async Task Cache_Memo_SecondQueryDoesNotHitCacheAgain()
    {
        Cache(100, new[] { 101 }, (1, true));
        Cache(101, Array.Empty<int>(), (2, true));
        var handler = NewHandler();

        await QueryAsync(handler, Id(100));
        var callsAfterFirst = _cacheGetListCalls;
        var second = await QueryAsync(handler, Id(100), Id(101));

        Assert.Equal(callsAfterFirst, _cacheGetListCalls);
        Assert.Equal(Sorted(new[] { Id(1), Id(2) }), Sorted(second));
    }

    [Fact]
    public async Task Cache_CyclicRoles_Terminate()
    {
        Cache(100, new[] { 101 }, (1, true));
        Cache(101, new[] { 100 }, (2, true));

        var result = await QueryAsync(NewHandler(), Id(100));

        Assert.Equal(Sorted(new[] { Id(1), Id(2) }), Sorted(result));
    }

    // ---------- DB 路径（缓存整体落空） ----------

    async Task<Guid> SeedParentWithChildAsync(bool parentRejectsPermission2)
    {
        var context = _db.Context;
        var child = new Role("child", "child", null, true, 0, RoleTypes.Domain);
        child.BindPermissions(new() { new SubjectPermissionRelationDto(Id(2), true), new SubjectPermissionRelationDto(Id(3), true) });
        context.Set<Role>().Add(child);
        await context.SaveChangesAsync();

        var parent = new Role("parent", "parent", null, true, 0, RoleTypes.Domain);
        parent.BindChildrenRoles(new() { child.Id });
        var permissions = new List<SubjectPermissionRelationDto> { new(Id(1), true) };
        if (parentRejectsPermission2)
            permissions.Add(new(Id(2), false));
        parent.BindPermissions(permissions);
        context.Set<Role>().Add(parent);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();
        _dbQueries.Count = 0;
        return parent.Id;
    }

    [Fact]
    public async Task Db_WholeCacheMiss_FallsBackToDb_AndResolvesChildren()
    {
        var parent = await SeedParentWithChildAsync(parentRejectsPermission2: false);

        var result = await QueryAsync(NewHandler(), parent);

        Assert.Equal(Sorted(new[] { Id(1), Id(2), Id(3) }), Sorted(result));
        Assert.True(_dbQueries.Count > 0);
        Assert.Equal(1, _cacheGetListCalls); //缓存只被整体查询了一次，DB 路径自给自足
    }

    [Fact]
    public async Task Db_ParentReject_FiltersChildPermission()
    {
        var parent = await SeedParentWithChildAsync(parentRejectsPermission2: true);

        var result = await QueryAsync(NewHandler(), parent);

        Assert.Equal(Sorted(new[] { Id(1), Id(3) }), Sorted(result));
    }

    sealed class CountingInterceptor : DbCommandInterceptor
    {
        public int Count { get; set; }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            Count++;
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            Count++;
            return new(result);
        }
    }
}
