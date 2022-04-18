namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class PermissionRepository : Repository<AuthDbContext, Permission, Guid>, IPermissionRepository
{
    public PermissionRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Permission> GetByIdAsync(Guid id)
    {
        return await Context.Set<Permission>()
            .Where(p => p.Id == id)
            .Include(p => p.Permissions)
            .Include(p => p.UserPermissions).ThenInclude(up => up.User)
            .Include(p => p.RolePermissions).ThenInclude(rp => rp.Role)
            .Include(p => p.TeamPermissions).ThenInclude(tp => tp.Team)
            .AsSplitQuery()
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current permission does not exist");
    }
}
