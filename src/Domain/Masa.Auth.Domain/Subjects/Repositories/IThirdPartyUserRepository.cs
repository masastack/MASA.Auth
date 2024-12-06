// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Subjects.Repositories;

public interface IThirdPartyUserRepository : IRepository<ThirdPartyUser>
{
    Task<ThirdPartyUser?> GetDetail(Guid id);
}
