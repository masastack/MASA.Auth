// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class StaffRepository : Repository<AuthDbContext, Staff, Guid>, IStaffRepository
{
    public StaffRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {

    }

    public async Task<Staff?> FindAsync(Expression<Func<Staff, bool>> predicate)
    {
        var staff = await Context.Set<Staff>()
                                .Include(s => s.DepartmentStaffs)
                                .Include(s => s.TeamStaffs)
                                .FirstOrDefaultAsync(predicate);

        return staff;
    }
}
