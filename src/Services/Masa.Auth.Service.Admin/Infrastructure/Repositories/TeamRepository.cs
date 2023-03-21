// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class TeamRepository : Repository<AuthDbContext, Team, Guid>, ITeamRepository
{
    public TeamRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public bool Any(Expression<Func<Team, bool>> predicate)
    {
        return Context.Set<Team>().Where(t => !t.IsDeleted).Any(predicate);
    }

    public async Task<Team> GetByIdAsync(Guid id)
    {
        return await Context.Set<Team>()
            .Where(t => t.Id == id)
            .Include(t => t.TeamPermissions)
            .Include(t => t.TeamStaffs)
            .Include(t => t.TeamRoles)
            .FirstOrDefaultAsync()
            ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.TEAM_NOT_EXIST);
    }

    public async Task<IReadOnlyList<Team>> GetAllAsync()
    {
        return await Context.Set<Team>()
            .Include(t => t.TeamPermissions)
            .Include(t => t.TeamStaffs)
            .Include(t => t.TeamRoles)
            .ToListAsync()
            ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.TEAM_NOT_EXIST);
    }
}
