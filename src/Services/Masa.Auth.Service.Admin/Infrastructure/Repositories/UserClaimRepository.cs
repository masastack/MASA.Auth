namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class UserClaimRepository : Repository<AuthDbContext, UserClaim, int>, IUserClaimRepository
{
    public UserClaimRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
