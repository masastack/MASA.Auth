// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class DbContextExtensions
{
    public static async Task<IEnumerable<T>> GetListInCludeAsync<T>(this AuthDbContext dbContext, Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<string>? includeProperties = null,
        CancellationToken cancellationToken = default(CancellationToken)) where T : Entity
    {
        var query = dbContext.Set<T>().Where(predicate);
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
