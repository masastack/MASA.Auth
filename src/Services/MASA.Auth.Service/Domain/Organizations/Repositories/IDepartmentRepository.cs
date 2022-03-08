using System.Linq.Expressions;

namespace Masa.Auth.Service.Domain.Organizations.Repositories;

public interface IDepartmentRepository : IRepository<Department, Guid>
{
    Task<Department> GetByIdAsync(Guid Id);

    Task<List<Department>> QueryListAsync(Expression<Func<Department, bool>> predicate);
}

