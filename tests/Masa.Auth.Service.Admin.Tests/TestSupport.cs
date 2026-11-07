// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Reflection;
using Masa.Auth.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// 与 PG 部署一致的实体配置 + SQLite 内存库（与 EntityFrameworkCore.Tests 同一套建库方式）。
/// </summary>
internal sealed class SqliteAuthDb : IDisposable
{
    readonly SqliteConnection _connection;

    public AuthDbContext Context { get; }

    public SqliteAuthDb(params IInterceptor[] interceptors)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        AuthDbContext.RegisterAssembly(typeof(Masa.Auth.EntityFrameworkCore.AuthPostgreSqlDbContextFactory).Assembly);
        AuthDbContext.RegisterAssembly(typeof(Masa.Contrib.Authentication.OpenIdConnect.EFCore.PostgreSql.OpenIdConnectEFPostgreSql).Assembly);

        var optionsBuilder = new MasaDbContextOptionsBuilder<AuthDbContext>();
        optionsBuilder.DbContextOptionsBuilder.UseSqlite(_connection).AddInterceptors(interceptors);
        Context = new AuthDbContext(optionsBuilder.MasaOptions);

        //DeviceFlowCodes 的唯一索引过滤器引用运行时才存在的 IsDeleted 影子属性，EnsureCreated 的 DDL 里要剔除（既有遗留问题，
        //生产走迁移不受影响；详见 EntityFrameworkCore.Tests）
        var script = Context.Database.GenerateCreateScript();
        var statements = script.Split(new[] { ";\r\n", ";\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Where(s => !(s.Contains("CREATE", StringComparison.OrdinalIgnoreCase)
                && s.Contains("INDEX", StringComparison.OrdinalIgnoreCase)
                && s.Contains("DeviceFlowCodes", StringComparison.OrdinalIgnoreCase)
                && s.Contains("IsDeleted", StringComparison.OrdinalIgnoreCase)));
        foreach (var statement in statements)
        {
            Context.Database.ExecuteSqlRaw(statement.Trim());
        }
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}

/// <summary>
/// 不引入 Mock 库的接口桩：未显式处理的方法返回"已完成的空结果"，需要真实行为的方法通过 <see cref="Override"/> 提供。
/// </summary>
public class StubProxy : DispatchProxy
{
    public Func<MethodInfo, object?[]?, (bool Handled, object? Result)>? Override { get; set; }

    public static T Create<T>(Func<MethodInfo, object?[]?, (bool Handled, object? Result)>? @override = null) where T : class
    {
        var proxy = Create<T, StubProxy>();
        ((StubProxy)(object)proxy).Override = @override;
        return proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (Override != null)
        {
            var (handled, result) = Override(targetMethod!, args);
            if (handled) return result;
        }
        return DefaultFor(targetMethod!.ReturnType);
    }

    static object? DefaultFor(Type type)
    {
        if (type == typeof(void)) return null;
        if (type == typeof(Task)) return Task.CompletedTask;
        if (type == typeof(ValueTask)) return default(ValueTask);
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var inner = type.GetGenericArguments()[0];
            return typeof(Task).GetMethod(nameof(Task.FromResult))!.MakeGenericMethod(inner).Invoke(null, new[] { MakeValue(inner) });
        }
        return MakeValue(type);
    }

    static object? MakeValue(Type type)
    {
        if (type.IsValueType) return Activator.CreateInstance(type);
        if (type.IsInterface || type.IsAbstract) return null;
        try { return Activator.CreateInstance(type, true); } catch { return null; }
    }
}
