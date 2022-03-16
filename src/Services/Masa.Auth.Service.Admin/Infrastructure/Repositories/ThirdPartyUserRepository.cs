using Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;
using Masa.Auth.Service.Admin.Domain.Subjects.Repositories;

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ThirdPartyUserRepository : Repository<AuthDbContext, ThirdPartyUser>, IThirdPartyUserRepository
{
    public ThirdPartyUserRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
