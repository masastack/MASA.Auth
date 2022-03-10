namespace Masa.Auth.Service.Infrastructure.Repositories;

public class TeamRepository : Repository<AuthDbContext, Team, Guid>, ITeamRepository
{
    public TeamRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
