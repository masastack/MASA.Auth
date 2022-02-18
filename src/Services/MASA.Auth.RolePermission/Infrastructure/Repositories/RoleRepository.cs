namespace MASA.Auth.RolePermission.Infrastructure.Repositories;

public class RoleRepository : Repository<RolePermissionDbContext, Role>, IRoleRepository
{

    public RoleRepository(RolePermissionDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}

