// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ClientRepository : Repository<AuthDbContext, Client, int>, IClientRepository
{
    public ClientRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Client> GetByIdAsync(int id)
    {
        return await Context.Set<Client>()
            .Where(c => c.Id == id)
            .Include(c => c.AllowedGrantTypes)
            .Include(c => c.RedirectUris)
            .Include(c => c.PostLogoutRedirectUris)
            .Include(c => c.Properties)
            .Include(c => c.Claims)
            .Include(c => c.IdentityProviderRestrictions)
            .Include(c => c.AllowedCorsOrigins)
            .Include(c => c.ClientSecrets)
            .Include(c => c.AllowedScopes)
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current client does not exist");
    }
}
