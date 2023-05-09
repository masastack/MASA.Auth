// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Repositories;

public interface IPermissionRepository : IRepository<Permission, Guid>
{
    Task<Permission> GetByIdAsync(Guid id);

    Task<List<Guid>> GetParentAsync(Guid id, bool recursive = true);

    int GetIncrementOrder(string appId, Guid parentId);

    bool Any(Expression<Func<Permission, bool>> predicate);

    List<string> GetPermissionCodes(Expression<Func<Permission, bool>> predicate);

    Task<List<Permission>> GetAllAsync();
}
