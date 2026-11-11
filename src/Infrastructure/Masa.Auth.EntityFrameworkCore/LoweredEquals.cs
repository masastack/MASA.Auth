// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore;

/// <summary>
/// LOWER(列) = lower(参数) 等值表达式的公共构造(应用层大小写不敏感等值,PG/SQLite/SS 语义一致)。
/// 注意:LOWER(列) 会让该列的普通索引失效(全表扫描)。SS 上已是 CI 排序规则的列(如 User.Account/Email)
/// 应走裸列比较以保留索引命中,由调用方自行分支(见 UserUniqueKeyQueries),不要盲目换成本构造;
/// 小表(如 IdentityProvider)则无此顾虑,直接用即可(见 IdentityProviderQueries)。
/// </summary>
internal static class LoweredEquals
{
    /// <summary>参数 value 必须已小写化(ToLowerInvariant),否则含大写的输入永远匹配不上。</summary>
    public static BinaryExpression Build(MemberExpression property, string? lowercasedValue)
        => Expression.Equal(
            Expression.Call(property, typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!),
            Expression.Constant(lowercasedValue));
}
