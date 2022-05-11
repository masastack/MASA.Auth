// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ApiScopeRepository : Repository<AuthDbContext, ApiScope, int>, IApiScopeRepository
{
    public ApiScopeRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<ApiScope?> GetDetailAsync(int id)
    {
        var apiScope = await Context.Set<ApiScope>()
                                .Include(apiScope => apiScope.UserClaims)
                                .Include(apiScope => apiScope.Properties)
                                .FirstOrDefaultAsync(apiScope => apiScope.Id == id);

        return apiScope;
    }
}
