using System.Linq.Expressions;

namespace Masa.Auth.Service.Infrastructure.Repositories;

public class DepartmentRepository : Repository<AuthDbContext, Department, Guid>, IDepartmentRepository
{
    public DepartmentRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Department> GetByIdAsync(Guid Id)
    {
        return await _context.Set<Department>()
            .Where(d => d.Id == Id)
            .Include(d => d.DepartmentStaffs)
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current department does not exist"); ;
    }

    public async Task<List<Department>> QueryListAsync(Expression<Func<Department, bool>> predicate)
    {
        var query = _context.Set<Department>().Where(predicate).Include(d => d.DepartmentStaffs);
        return await query.ToListAsync();
    }
}
