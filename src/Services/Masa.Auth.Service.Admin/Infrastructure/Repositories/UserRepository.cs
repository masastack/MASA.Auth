namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class UserRepository : Repository<AuthDbContext, User>, IUserRepository
{
    public UserRepository(AuthDbContext context, IUnitOfWork intOfWork) : base(context, intOfWork)
    {
    }
}
