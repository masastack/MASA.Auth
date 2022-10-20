// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ThirdPartyUserRepository : Repository<AuthDbContext, ThirdPartyUser>, IThirdPartyUserRepository
{
    public ThirdPartyUserRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<ThirdPartyUser?> GetDetail(Guid id)
    {
        return await Context.Set<ThirdPartyUser>()
                           .Where(s => s.Id == id)
                           .FirstOrDefaultAsync();
    }
}
