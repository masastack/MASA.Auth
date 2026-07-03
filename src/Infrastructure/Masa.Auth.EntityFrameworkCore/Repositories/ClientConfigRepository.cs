// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.Repositories;

public class ClientConfigRepository : Repository<AuthDbContext, ClientConfig, int>, IClientConfigRepository
{
    public ClientConfigRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<ClientConfig?> FindByClientIdAsync(string clientId)
    {
        return await Context.Set<ClientConfig>()
            .Include(x => x.MessageTemplates)
            .AsTracking()
            .FirstOrDefaultAsync(x => x.ClientId == clientId);
    }
}
