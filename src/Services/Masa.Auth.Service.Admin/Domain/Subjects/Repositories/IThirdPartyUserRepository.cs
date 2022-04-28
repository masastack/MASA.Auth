namespace Masa.Auth.Service.Admin.Domain.Subjects.Repositories;

public interface IThirdPartyUserRepository : IRepository<ThirdPartyUser>
{
    Task<ThirdPartyUser?> GetDetail(Guid id);
}
