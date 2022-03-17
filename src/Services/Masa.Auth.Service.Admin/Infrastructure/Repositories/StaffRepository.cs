namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class StaffRepository : Repository<AuthDbContext, Staff, Guid>, IStaffRepository
{
    public StaffRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
