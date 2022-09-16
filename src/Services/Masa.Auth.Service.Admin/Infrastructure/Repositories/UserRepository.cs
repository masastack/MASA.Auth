// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class UserRepository : Repository<AuthDbContext, User>, IUserRepository
{

    public UserRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public bool Any(Expression<Func<User, bool>> predicate)
    {
        return Context.Set<User>().Where(d => !d.IsDeleted).Any(predicate);
    }

    public Task<User?> FindWithIncludAsync(Expression<Func<User, bool>> predicate, List<string>? includeProperties = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Set<User>().Where(predicate);
        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await Context.Set<User>().ToListAsync();
    }

    public async Task<User?> GetDetailAsync(Guid id)
    {
        var user = await Context.Set<User>()
                           .Include(u => u.Roles)
                           .Include(u => u.Permissions)
                           .Include(u => u.ThirdPartyUsers)
                           .FirstOrDefaultAsync(u => u.Id == id);

        return user;
    }
}
