namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class PositionRepository : Repository<AuthDbContext, Position, Guid>, IPositionRepository
{
    public PositionRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
