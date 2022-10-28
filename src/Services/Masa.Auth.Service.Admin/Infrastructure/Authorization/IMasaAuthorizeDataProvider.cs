// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Authorization;

public interface IMasaAuthorizeDataProvider : IScopedDependency
{
    Task<string> GetAccountAsync();

    Task<IEnumerable<string>> GetAllowCodeListAsync(string appId, Guid userId);
}
