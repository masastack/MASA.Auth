namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class AppNavigationTagRepository : Repository<AuthDbContext, AppNavigationTag, Guid>, IAppNavigationTagRepository
{
    public AppNavigationTagRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
