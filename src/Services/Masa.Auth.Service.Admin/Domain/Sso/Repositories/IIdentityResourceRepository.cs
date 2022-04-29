namespace Masa.Auth.Service.Admin.Domain.Sso.Repositories;

public interface IIdentityResourceRepository : IRepository<IdentityResource, int>
{
    Task<IdentityResource?> GetDetailAsync(int id);
}
