// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class DepartmentRepository : Repository<AuthDbContext, Department, Guid>, IDepartmentRepository
{
    public DepartmentRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Department> GetByIdAsync(Guid id)
    {
        return await Context.Set<Department>()
            .Where(d => d.Id == id)
            .Include(d => d.DepartmentStaffs)
            .ThenInclude(ds => ds.Staff)
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current department does not exist");
    }

    public async Task<List<Department>> QueryListAsync(Expression<Func<Department, bool>> predicate)
    {
        var query = Context.Set<Department>().Where(predicate).Include(d => d.DepartmentStaffs);
        return await query.ToListAsync();
    }
}
