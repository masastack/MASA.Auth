// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Repositories;

public interface IIdentityResourceRepository : IRepository<IdentityResource, int>
{
    Task<IdentityResource?> GetDetailByIdAsync(int id);

    Task<List<IdentityResourceSelectDto>> GetIdentityResourceSelect();
}
