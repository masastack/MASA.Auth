namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class IdentityResourceRepository : Repository<AuthDbContext, IdentityResource, int>, IIdentityResourceRepository
{
    public IdentityResourceRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<IdentityResource?> GetDetailAsync(int id)
    {
        var idrs = await Context.Set<IdentityResource>()
                                .Include(idrs => idrs.UserClaims)
                                .Include(idrs => idrs.Properties)
                                .FirstOrDefaultAsync(idrs => idrs.Id == id);

        return idrs;
    }
}
