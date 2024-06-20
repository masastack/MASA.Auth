// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class GlobalNavVisibleRepository : Repository<AuthDbContext, GlobalNavVisible, Guid>, IGlobalNavVisibleRepository
{
    public GlobalNavVisibleRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<List<string>> GetAppIds(Expression<Func<GlobalNavVisible, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<GlobalNavVisible>().Where(predicate).Select(x => x.AppId).ToListAsync(cancellationToken);
    }
}
