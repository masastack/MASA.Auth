namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class RoleRepository : Repository<AuthDbContext, Role, Guid>, IRoleRepository
{
    public RoleRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Role> GetByIdAsync(Guid id)
    {
        return await Context.Set<Role>()
            .Where(r => r.Id == id)
            .Include(r => r.Roles)
            .Include(r => r.RolePermissions)
            .AsSplitQuery()
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current role does not exist");
    }
}
