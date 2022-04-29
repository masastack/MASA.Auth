namespace Masa.Auth.Service.Admin.Domain.Sso.Repositories;

public interface IApiResourceRepository : IRepository<ApiResource, int>
{
    Task<ApiResource?> GetDetailAsync(int id);
}
