namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class TeamRepository : Repository<AuthDbContext, Team, Guid>, ITeamRepository
{
    public TeamRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Team> GetByIdAsync(Guid id)
    {
        return await _context.Set<Team>()
            .Where(t => t.Id == id)
            .Include(t => t.Permissions)
            .Include(t => t.Staffs)
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current team does not exist");
    }
}
