using Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;
using System.Linq.Expressions;

namespace Masa.Auth.Service.Admin.Domain.Organizations.Repositories;

public interface IDepartmentRepository : IRepository<Department, Guid>
{
    Task<Department> GetByIdAsync(Guid id);

    Task<List<Department>> QueryListAsync(Expression<Func<Department, bool>> predicate);
}

