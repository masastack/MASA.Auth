namespace Masa.Auth.Service.Domain.Permissions.Repositories;

public interface IPermissionRepository : IRepository<Permission, Guid>
{
    Task<Permission> GetByIdAsync(Guid Id);
}
