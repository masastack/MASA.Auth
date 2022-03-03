namespace MASA.Auth.Service.Infrastructure.Repositories;

public class DepartmentRepository : Repository<AuthDbContext, Department>, IDepartmentRepository
{
    public DepartmentRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}

