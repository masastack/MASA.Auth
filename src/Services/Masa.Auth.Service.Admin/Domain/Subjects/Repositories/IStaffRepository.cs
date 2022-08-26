// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Repositories;

public interface IStaffRepository : IRepository<Staff, Guid>
{
    Task<Staff?> FindAsync(Expression<Func<Staff, bool>> predicate);

    Task<Staff?> GetDetailById(Guid id);
}
