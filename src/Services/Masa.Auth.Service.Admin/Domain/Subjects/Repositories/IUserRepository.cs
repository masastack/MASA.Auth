// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetDetailAsync(Guid id);

    Task<User?> FindWithIncludAsync(Expression<Func<User, bool>> predicate, List<string>? includeProperties = null, CancellationToken cancellationToken = default(CancellationToken));

    Task<List<User>> GetAllAsync();

    bool Any(Expression<Func<User, bool>> predicate);
}
