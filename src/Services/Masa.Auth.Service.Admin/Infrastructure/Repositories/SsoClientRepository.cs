namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class SsoClientRepository : Repository<AuthDbContext, Client, int>, ISsoClientRepository
{
    public SsoClientRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
