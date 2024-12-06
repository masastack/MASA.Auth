// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.EntityFrameworkCore.Repositories;

public class UserSystemBusinessDataRepository : Repository<AuthDbContext, UserSystemBusinessData>, IUserSystemBusinessDataRepository
{
    public UserSystemBusinessDataRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
