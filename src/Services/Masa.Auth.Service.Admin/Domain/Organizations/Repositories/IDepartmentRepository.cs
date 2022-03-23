namespace Masa.Auth.Service.Admin.Domain.Organizations.Repositories;

public interface IdepartmentRepository : IRepository<Department, Guid>
{
    Task<Department> GetByIdAsync(Guid id);

    Task<List<Department>> QueryListAsync(Expression<Func<Department, bool>> predicate);
}

