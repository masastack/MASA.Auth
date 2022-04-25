namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class UserRepository : Repository<AuthDbContext, User>, IUserRepository
{
    public UserRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<User?> GetDetail(Guid id)
    {
        var user = await Context.Set<User>()
                           .Include(u => u.Roles)
                           .Include(u => u.Permissions)
                           .Include(u => u.ThirdPartyUsers)
                           .FirstOrDefaultAsync(u => u.Id == id);

        return user;
    }
}
