namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ThirdPartyIdpRepository : Repository<AuthDbContext, ThirdPartyIdp>, IThirdPartyIdpRepository
{
    public ThirdPartyIdpRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
