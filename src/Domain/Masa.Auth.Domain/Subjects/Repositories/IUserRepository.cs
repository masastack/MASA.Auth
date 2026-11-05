// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Subjects.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetDetailAsync(Guid id);

    Task<User?> FindWithIncludAsync(Expression<Func<User, bool>> predicate, List<string>? includeProperties = null, CancellationToken cancellationToken = default(CancellationToken));

    Task<List<User>> GetAllAsync();

    bool Any(Expression<Func<User, bool>> predicate);

    Task<User> GetByVoucherAsync(string voucher);

    IQueryable<User> AsQueryable();

    /// <summary>
    /// 是否存在 Account 等于指定值的用户(统一大小写不敏感口径,排除软删)。
    /// 语义等价于 Any(UserUniqueKeyQueries.AccountEquals(...)),调用方不应再自行构造该等值条件。
    /// </summary>
    Task<bool> AnyAccountEqualsAsync(string? account, CancellationToken cancellationToken = default(CancellationToken));

    /// <summary>
    /// 是否存在 Email 等于指定值的用户(统一大小写不敏感口径,排除软删;PG 分支额外排除空串 Email)。
    /// 语义等价于 Any(UserUniqueKeyQueries.EmailEquals(...)),调用方不应再自行构造该等值条件。
    /// </summary>
    Task<bool> AnyEmailEqualsAsync(string? email, CancellationToken cancellationToken = default(CancellationToken));
}
