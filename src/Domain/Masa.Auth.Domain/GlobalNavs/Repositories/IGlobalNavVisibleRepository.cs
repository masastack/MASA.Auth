// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.GlobalNavs.Repositories;

public interface IGlobalNavVisibleRepository : IRepository<GlobalNavVisible, Guid>
{
    Task<List<string>> GetAppIds(Expression<Func<GlobalNavVisible, bool>> predicate, CancellationToken cancellationToken = default);
}
