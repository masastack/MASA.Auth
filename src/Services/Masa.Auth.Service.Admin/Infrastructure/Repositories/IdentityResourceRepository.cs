namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class IdentityResourceRepository : Repository<AuthDbContext, IdentityResource, int>, IIdentityResourceRepository
{
    public IdentityResourceRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<IdentityResource?> GetDetailByIdAsync(int id)
    {
        var idrs = await Context.Set<IdentityResource>()
                                .Include(idrs => idrs.UserClaims)
                                .Include(idrs => idrs.Properties)
                                .FirstOrDefaultAsync(idrs => idrs.Id == id);

        return idrs;
    }

    public async Task<List<IdentityResourceSelectDto>> GetIdentityResourceSelect()
    {
        var idrs = await Context.Set<IdentityResource>()
                                .Select(idrs => new IdentityResourceSelectDto(idrs.Id, idrs.Name, idrs.DisplayName, idrs.Description))
                                .ToListAsync();

        return idrs;
    }
}
