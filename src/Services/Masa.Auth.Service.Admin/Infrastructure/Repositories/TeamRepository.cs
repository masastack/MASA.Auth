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
        return Context.Set<Team>().Any(predicate);
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

    public async Task<IEnumerable<Team>> GetListInCludeAsync(Expression<Func<Team, bool>> predicate, Func<IQueryable<Team>, IOrderedQueryable<Team>>? orderBy = null,
            List<string>? includeProperties = null, CancellationToken cancellationToken = default)
    {
        var query = Context.Set<Team>().Where(predicate);
        if (orderBy != null)
        {
            query = orderBy(query);
        }
        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.ToListAsync(cancellationToken);
    }
}
