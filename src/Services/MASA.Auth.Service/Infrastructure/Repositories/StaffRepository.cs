namespace MASA.Auth.Service.Infrastructure.Repositories;

public class StaffRepository : Repository<AuthDbContext, Staff>, IStaffRepository
{
    public StaffRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
