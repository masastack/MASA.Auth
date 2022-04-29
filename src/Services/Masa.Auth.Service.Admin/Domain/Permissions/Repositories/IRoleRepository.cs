// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Repositories;

public interface IRoleRepository : IRepository<Role, Guid>
{
    Task<Role> GetByIdAsync(Guid Id);

    Task<Role> GetDetailAsync(Guid id);
}
