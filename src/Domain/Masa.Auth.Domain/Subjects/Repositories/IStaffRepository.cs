﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Subjects.Repositories;

public interface IStaffRepository : IRepository<Staff, Guid>
{
    Task<Staff?> FindAsync(Expression<Func<Staff, bool>> predicate);

    Task<Staff> GetDetailByIdAsync(Guid id);

    Task<Staff> GetByUserIdAsync(Guid userId);
}
