namespace Masa.Auth.Service.Admin.Domain.Sso.Repositories;

public interface IIdentityResourceRepository : IRepository<IdentityResource, int>
{
    Task<IdentityResource?> GetDetailByIdAsync(int id);

    Task<List<IdentityResourceSelectDto>> GetIdentityResourceSelect();
}
