namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class TeamRepository : Repository<AuthDbContext, Team, Guid>, ITeamRepository
{
    public TeamRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<Team> GetByIdAsync(Guid id)
    {
        return await Context.Set<Team>()
            .Where(t => t.Id == id)
            .Include(t => t.TeamPermissions)
            .Include(t => t.TeamStaffs)
            .Include(t => t.TeamRoles)
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException("The current team does not exist");
    }

    public override Task<Team> RemoveAsync(Team entity, CancellationToken cancellationToken = default(CancellationToken))
    {
#warning temporary
        Context.Entry(entity).Property<bool>("IsDeleted").CurrentValue = true;
        Context.SaveChanges();
        return Task.FromResult(entity);
    }
}
