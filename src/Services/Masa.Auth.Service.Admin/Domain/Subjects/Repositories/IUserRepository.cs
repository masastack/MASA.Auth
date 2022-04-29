﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetDetail(Guid id);
}
