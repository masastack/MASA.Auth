// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.Repositories;

public class UserClaimExtendRepository : Repository<AuthDbContext, UserClaimExtend, int>, IUserClaimExtendRepository
{
    public UserClaimExtendRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}

