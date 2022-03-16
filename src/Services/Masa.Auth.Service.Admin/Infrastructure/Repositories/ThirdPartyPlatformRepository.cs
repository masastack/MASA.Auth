using Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;
using Masa.Auth.Service.Admin.Domain.Subjects.Repositories;
using Masa.Auth.Service.Admin.Infrastructure;

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ThirdPartyPlatformRepository : Repository<AuthDbContext, ThirdPartyIdp>, IThirdPartyPlatformRepository
{
    public ThirdPartyPlatformRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
