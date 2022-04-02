namespace Masa.Auth.Service.Admin.Domain.Permissions.Repositories;

public interface IRoleRepository : IRepository<Role, Guid>
{
    Task<Role> GetByIdAsync(Guid Id);
}
