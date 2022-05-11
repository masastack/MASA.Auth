// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class PermissionRepository : Repository<AuthDbContext, Permission, Guid>, IPermissionRepository
{
    public PermissionRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Permission> GetByIdAsync(Guid id)
    {
        return await Context.Set<Permission>()
            .Where(p => p.Id == id)
            .Include(p => p.ChildPermissionRelations)
            .Include(p => p.ParentPermissionRelations)
            .Include(p => p.UserPermissions).ThenInclude(up => up.User)
            .Include(p => p.RolePermissions).ThenInclude(rp => rp.Role)
            .Include(p => p.TeamPermissions).ThenInclude(tp => tp.Team)
            .AsSplitQuery()
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current permission does not exist");
    }
}
