// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class IdentityResourceRepository : Repository<AuthDbContext, IdentityResource, int>, IIdentityResourceRepository
{
    public IdentityResourceRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<IdentityResource?> GetDetailAsync(int id)
    {
        var idrs = await Context.Set<IdentityResource>()
                                .Include(idrs => idrs.UserClaims)
                                .Include(idrs => idrs.Properties)
                                .FirstOrDefaultAsync(idrs => idrs.Id == id);

        return idrs;
    }
}
