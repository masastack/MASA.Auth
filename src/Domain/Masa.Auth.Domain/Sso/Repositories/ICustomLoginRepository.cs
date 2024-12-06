// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Sso.Repositories;

public interface ICustomLoginRepository : IRepository<CustomLogin, int>
{
    Task<CustomLogin?> GetDetailAsync(int id);
}
