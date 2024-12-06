// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.Repositories;

public class DepartmentRepository : Repository<AuthDbContext, Department, Guid>, IDepartmentRepository
{
    public DepartmentRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public bool Any(Expression<Func<Department, bool>> predicate)
    {
        return Context.Set<Department>().Where(d => !d.IsDeleted).Any(predicate);
    }

    public async Task<Department> GetByIdAsync(Guid id)
    {
        return await Context.Set<Department>()
            .Where(d => d.Id == id)
            .Include(d => d.DepartmentStaffs)
            .ThenInclude(ds => ds.Staff)
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.DEPARTMENT_NOT_EXIST);
    }

    public async Task<List<Department>> QueryListAsync(Expression<Func<Department, bool>> predicate)
    {
        var query = Context.Set<Department>().Where(predicate).Include(d => d.DepartmentStaffs);
        return await query.ToListAsync();
    }

    public Dictionary<int, int> LevelQuantity()
    {
        return Context.Set<Department>().GroupBy(d => d.Level).Select(g => new
        {
            g.Key,
            Count = g.Count()
        }).ToDictionary(d => d.Key, d => d.Count);
    }
}
