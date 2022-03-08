using Masa.Auth.Service.Domain.Organizations.Repositories;

namespace Masa.Auth.Service.Infrastructure.Repositories;

public class DepartmentRepository : Repository<AuthDbContext, Department, Guid>, IDepartmentRepository
{
    public DepartmentRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}

