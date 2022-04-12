namespace Masa.Auth.Service.Admin.Domain.Subjects.Repositories;

public interface IStaffRepository : IRepository<Staff, Guid>
{
    Task<Staff?> FindAsync(Expression<Func<Staff, bool>> predicate);
}
