// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore;

/// <summary>
/// User.Account / User.Email 的等值查询统一按不区分大小写口径构造(应用层语义)。
/// PG 分支生成 LOWER(列) = lower(参数)——正确但不命中普通索引(PG 的 DB 层唯一索引为
/// 大小写敏感,仅作完全重复的兜底,见 UserEntityTypeConfiguration 注释),登录/注册等
/// 查找为全表扫描,属已接受的代价;SS 分支裸列比较,CI 排序规则下直接走索引。
/// 涉及这两列的等值条件统一从这里构造,不要在查询处散落 <c>Account == x</c>。
/// LOWER 等值表达式本身的构造见 <see cref="LoweredEquals"/>。
/// </summary>
public static class UserUniqueKeyQueries
{
    const string NpgsqlProviderName = "Npgsql.EntityFrameworkCore.PostgreSQL";

    public static Expression<Func<User, bool>> AccountEquals(AuthDbContext dbContext, string? account)
        => TextEquals(dbContext, user => user.Account, account, withNonEmptyGuard: false);

    public static Expression<Func<User, bool>> EmailEquals(AuthDbContext dbContext, string? email)
        //Email 允许为空串(非唯一);PG 的 LOWER() 预检查带上 Email != '' 与应用层语义一致
        => TextEquals(dbContext, user => user.Email, email, withNonEmptyGuard: true);

    static Expression<Func<User, bool>> TextEquals(AuthDbContext dbContext, Expression<Func<User, string>> selector, string? value, bool withNonEmptyGuard)
    {
        var parameter = selector.Parameters[0];
        var property = (MemberExpression)selector.Body;
        //参数必须小写化:LOWER(列)/CI 比较的参照值是小写,否则含大写的输入永远匹配不上
        var normalized = value?.ToLowerInvariant();
        Expression body;
        if (dbContext.Database.ProviderName == NpgsqlProviderName)
        {
            body = LoweredEquals.Build(property, normalized);
            if (withNonEmptyGuard)
            {
                body = Expression.AndAlso(body, Expression.NotEqual(property, Expression.Constant(string.Empty)));
            }
        }
        else
        {
            //SS 列排序规则为 CI,裸列比较即不区分大小写且走索引;参数小写化对 CI 比较无影响
            body = Expression.Equal(property, Expression.Constant(normalized));
        }
        return Expression.Lambda<Func<User, bool>>(body, parameter);
    }
}
