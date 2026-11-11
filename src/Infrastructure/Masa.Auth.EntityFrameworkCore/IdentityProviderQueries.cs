// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore;

/// <summary>
/// IdentityProvider.Name(登录 scheme)的等值查询/查重统一按不区分大小写口径构造(应用层语义),
/// 与 SSO 侧 RemoteAuthenticationDefaultsProvider 的 OrdinalIgnoreCase 匹配对齐,消除
/// "SSO 放行、Admin 精确匹配查空"的分层不一致(IDENTITY_PROVIDER_NOT_FOUND)。
/// 两个 provider 都生成 LOWER(列) = lower(参数):PG 默认 collation 大小写敏感;SS 侧 Name 列
/// 无 CI 排序规则(库默认 CS_AS),裸列比较同样敏感——故与 UserUniqueKeyQueries 不同,不按
/// provider 分支。IdentityProvider 表行数极少,LOWER() 不命中普通索引属可忽略的代价。
/// DB 层 Name 过滤唯一索引为大小写敏感,仅挡完全重复;大小写变体的唯一性由这里的等值条件
/// 在应用层承担(先查后插,并发窗口可能漏过,与 Account/Email 同为已接受的取舍)。
/// 涉及该列的等值条件统一从这里构造,不要在查询处散落 <c>Name == x</c>。
/// 泛型 T 用于适配不同消费方的表达式签名:Set、ILdapIdpRepository、IThirdPartyIdpRepository
/// 各自要求自己实体类型的 Expression(Expression 不变型,基类表达式无法直接传给派生类型仓储);
/// 表达式构造见 <see cref="LoweredEquals"/>。
/// </summary>
public static class IdentityProviderQueries
{
    public static Expression<Func<T, bool>> NameEquals<T>(string? name) where T : IdentityProvider
    {
        //参数必须小写化:LOWER(列) 比较的参照值是小写,否则含大写的输入永远匹配不上
        var parameter = Expression.Parameter(typeof(T), "idp");
        var property = Expression.Property(parameter, nameof(IdentityProvider.Name));
        var body = LoweredEquals.Build(property, name?.ToLowerInvariant());
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
