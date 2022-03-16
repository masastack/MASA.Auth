namespace Masa.Auth.Service.Infrastructure.Repositories;

public class ThirdPartyIdpRepository : Repository<AuthDbContext, ThirdPartyIdp>, IThirdPartyIdpRepository
{
    public ThirdPartyIdpRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
