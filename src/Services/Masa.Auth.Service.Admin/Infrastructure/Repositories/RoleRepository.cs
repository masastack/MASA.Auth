namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class RoleRepository : Repository<AuthDbContext, Role, Guid>, IRoleRepository
{
    public RoleRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
