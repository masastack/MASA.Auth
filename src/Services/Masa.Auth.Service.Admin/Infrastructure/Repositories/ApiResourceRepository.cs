﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ApiResourceRepository : Repository<AuthDbContext, ApiResource, int>, IApiResourceRepository
{
    public ApiResourceRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<ApiResource?> GetDetailAsync(int id)
    {
        var apiResource = await Context.Set<ApiResource>()
                                .Include(apiResource => apiResource.UserClaims)
                                .Include(apiResource => apiResource.Properties)
                                .Include(apiResource => apiResource.ApiScopes)
                                .FirstOrDefaultAsync(apiResource => apiResource.Id == id);

        return apiResource;
    }
}
