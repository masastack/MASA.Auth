namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ApiScopeRepository : Repository<AuthDbContext, ApiScope, int>, IApiScopeRepository
{
    public ApiScopeRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<ApiScope?> GetDetailAsync(int id)
    {
        var apiScope = await Context.Set<ApiScope>()
                                .Include(apiScope => apiScope.UserClaims)
                                .Include(apiScope => apiScope.Properties)
                                .FirstOrDefaultAsync(apiScope => apiScope.Id == id);

        return apiScope;
    }
}
