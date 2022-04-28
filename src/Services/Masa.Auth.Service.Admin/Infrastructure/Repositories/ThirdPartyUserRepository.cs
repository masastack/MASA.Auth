namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ThirdPartyUserRepository : Repository<AuthDbContext, ThirdPartyUser>, IThirdPartyUserRepository
{
    public ThirdPartyUserRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<ThirdPartyUser?> GetDetail(Guid id)
    {
        return await Context.Set<ThirdPartyUser>()
                           .Include(s => s.CreateUser)
                           .Include(s => s.ModifyUser)
                           .Where(s => s.Id == id)
                           .FirstOrDefaultAsync();
    }
}
