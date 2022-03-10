namespace Masa.Auth.Service.Infrastructure.Repositories;

public class PermissionRepository : Repository<AuthDbContext, Permission, Guid>, IPermissionRepository
{
    public PermissionRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Permission> GetByIdAsync(Guid id)
    {
        return await _context.Set<Permission>()
            .Where(p => p.Id == id)
            .Include(p => p.UserPermissions)
            .Include(p => p.RolePermissions)
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current permission does not exist");
    }
}
