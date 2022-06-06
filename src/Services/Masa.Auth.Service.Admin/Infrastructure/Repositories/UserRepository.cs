// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Caching.DistributedMemory.Interfaces;

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class UserRepository : Repository<AuthDbContext, User>, IUserRepository
{
    readonly IMemoryCacheClient _memoryCacheClient;

    public UserRepository(AuthDbContext context, IUnitOfWork unitOfWork, IMemoryCacheClient memoryCacheClient) : base(context, unitOfWork)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await Context.Set<User>().ToListAsync();
    }

    public async Task<User?> GetDetailAsync(Guid id)
    {
        var user = await Context.Set<User>()
                           .Include(u => u.Roles)
                           .Include(u => u.Permissions)
                           .Include(u => u.ThirdPartyUsers)
                           .FirstOrDefaultAsync(u => u.Id == id);

        return user;
    }
}
