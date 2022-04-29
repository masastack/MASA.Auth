namespace Masa.Auth.Service.Admin.Domain.Sso.Repositories;

public interface IApiScopeRepository : IRepository<ApiScope, int>
{
    Task<ApiScope?> GetDetailAsync(int id);
}
