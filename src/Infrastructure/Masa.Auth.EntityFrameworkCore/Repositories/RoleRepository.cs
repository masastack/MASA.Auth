﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.Repositories;

public class RoleRepository : Repository<AuthDbContext, Role, Guid>, IRoleRepository
{
    public RoleRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Role> GetByIdAsync(Guid id)
    {
        return await Context.Set<Role>()
            .Where(r => r.Id == id)
            .Include(r => r.ChildrenRoles)
            .Include(r => r.Permissions)
            .Include(r => r.Clients)
            .AsSplitQuery()
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ROLE_NOT_EXIST);
    }

    public async Task<Role> GetDetailAsync(Guid id)
    {
        return await Context.Set<Role>()
            .Where(r => r.Id == id)
            .Include(r => r.ParentRoles)
            .Include(r => r.ChildrenRoles)
            .Include(r => r.Permissions)
            .Include(r => r.Users)
            .ThenInclude(u => u.User)
            .Include(r => r.Teams)
            .Include(r => r.Clients)
            .AsSplitQuery()
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ROLE_NOT_EXIST);
    }

    public async Task<Role> GetWithUsersAsync(Guid id)
    {
        return await Context.Set<Role>()
            .Where(r => r.Id == id)
            .Include(r => r.Users)
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ROLE_NOT_EXIST);
    }

    public async Task<List<Role>> GetListWithPermissionsAsync()
    {
        return await Context.Set<Role>()
            .Include(r => r.ChildrenRoles)
            .Include(r => r.Permissions)
            .AsSplitQuery()
            .ToListAsync();
    }
}
