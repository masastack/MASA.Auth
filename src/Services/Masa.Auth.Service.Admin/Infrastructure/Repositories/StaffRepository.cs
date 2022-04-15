namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class StaffRepository : Repository<AuthDbContext, Staff, Guid>, IStaffRepository
{
    public StaffRepository(AuthDbContext context, IUnitOfWork intOfWork) : base(context, intOfWork)
    {

    }

    public async Task<Staff?> FindAsync(Expression<Func<Staff, bool>> predicate)
    {
        var staff = await Context.Set<Staff>().Include(s => s.DepartmentStaffs)
                                        .Include(s => s.TeamStaffs)
                                        .Include(s => s.Position)
                                        .FirstOrDefaultAsync(predicate);

        return staff;
    }
}
