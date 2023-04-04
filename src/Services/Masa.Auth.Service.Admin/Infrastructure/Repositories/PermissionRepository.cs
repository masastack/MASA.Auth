// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class PermissionRepository : Repository<AuthDbContext, Permission, Guid>, IPermissionRepository
{
    public PermissionRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public bool Any(Expression<Func<Permission, bool>> predicate)
    {
        return Context.Set<Permission>().Where(d => !d.IsDeleted).Any(predicate);
    }

    public async Task<List<Permission>> GetAllAsync()
    {
        return await Context.Set<Permission>().Where(u => !u.IsDeleted)
            .Include(p => p.ChildPermissionRelations)
            .Include(p => p.ParentPermissionRelations)
            .AsNoTracking()
            .ToListAsync();
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
            ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSION_NOT_EXIST);
    }

    public int GetIncrementOrder(string appId, Guid parentId)
    {
        var maxOrder = Context.Set<Permission>()
            .Where(p => p.ParentId == parentId && p.AppId == appId)
            .Select(p => p.Order)
            .DefaultIfEmpty()
            .Max();
        return Math.Min(maxOrder + 1, BusinessConsts.PERMISSION_ORDER_MAX_VALUE);
    }

    public async Task<List<Guid>> GetParentAsync(Guid Id, bool recursive = true)
    {
        var result = new List<Guid>();
        var item = await Context.Set<Permission>().FindAsync(Id);
        if (item is null)
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.PERMISSIION_NOT_FOUND);
        }
        if (item.ParentId == Guid.Empty)
        {
            return new();
        }
        result.Add(item.ParentId);
        if (!recursive)
        {
            return result;
        }
        result.AddRange(await GetParentAsync(item.ParentId, recursive));
        return result;
    }

    public List<string> GetPermissionCodes(Expression<Func<Permission, bool>> predicate)
    {
        return Context.Set<Permission>().Where(predicate).Select(p => p.ReplenishCode).ToList();
    }
}
