namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class RoleRepository : Repository<AuthDbContext, Role, Guid>, IRoleRepository
{
    public RoleRepository(AuthDbContext context, IUnitOfWork intOfWork) : base(context, intOfWork)
    {
    }

    public async Task<Role> GetByIdAsync(Guid id)
    {
        return await Context.Set<Role>()
            .Where(r => r.Id == id)
            .Include(r => r.ChildrenRoles)
            .Include(r => r.Permissions)           
            .AsSplitQuery()
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current role does not exist");
    }

    public async Task<Role> GetDetailAsync(Guid id)
    {
        return await Context.Set<Role>()
            .Where(r => r.Id == id)
            .Include(r => r.ChildrenRoles)
            .ThenInclude(cr => cr.Role)
            .Include(r => r.Permissions)
            .Include(r => r.Users)
            .AsSplitQuery()
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current role does not exist");
    }

    public async Task<List<Role>> GetListAsync()
    {
        return await Context.Set<Role>()
                            .Where(r => r.Hidden == false)
                            .ToListAsync();
    }
}
