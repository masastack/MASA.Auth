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
        var result = new List<User>();
        var pageSize = 5000;
        for (int i = 0; i < 50; i++)
        {
            var users = await Context.Set<User>().Where(u => !u.IsDeleted).Skip((i - 1) * pageSize).Take(pageSize)
            .Include(u => u.Roles)
            .ThenInclude(ur => ur.Role)
            .Include(u => u.Permissions)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();
            if (users.Count == 0)
                break;
            result.AddRange(users);
        }

        return result;
    }

    public async Task<User> GetByVoucherAsync(string voucher)
    {
        return await Context.Set<User>().FirstOrDefaultAsync(u => u.Account == voucher
            || u.PhoneNumber == voucher || u.Email == voucher) ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_FOUND);
    }

    public async Task<User> GetDetailAsync(Guid id)
    {
        var user = await Context.Set<User>()
                           .Include(u => u.Roles)
                           .ThenInclude(ur => ur.Role)
                           .Include(u => u.Permissions)
                           .Include(u => u.ThirdPartyUsers)
                           .Include(u => u.Staff)
                           .FirstOrDefaultAsync(u => u.Id == id);

        return user ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
    }
}
