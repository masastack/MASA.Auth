// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Organizations.Repositories;

public interface IDepartmentRepository : IRepository<Department, Guid>
{
    Task<Department> GetByIdAsync(Guid id);

    Task<List<Department>> QueryListAsync(Expression<Func<Department, bool>> predicate);

    bool Any(Expression<Func<Department, bool>> predicate);

    Dictionary<int, int> LevelQuantity();
}

