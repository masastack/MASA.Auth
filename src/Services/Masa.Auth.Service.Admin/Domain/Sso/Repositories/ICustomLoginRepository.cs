namespace Masa.Auth.Service.Admin.Domain.Sso.Repositories;

public interface ICustomLoginRepository : IRepository<CustomLogin, int>
{
    Task<CustomLogin?> GetDetailAsync(int id);
}
