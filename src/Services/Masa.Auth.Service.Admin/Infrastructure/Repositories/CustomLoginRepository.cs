﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class CustomLoginRepository : Repository<AuthDbContext, CustomLogin, int>, ICustomLoginRepository
{
    public CustomLoginRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task<CustomLogin?> GetDetailAsync(int id)
    {
        var customLogin = await Context.Set<CustomLogin>()
                                .Include(customLogin => customLogin.ThirdPartyIdps)
                                .Include(customLogin => customLogin.RegisterFields)
                                .AsTracking()
                                .FirstOrDefaultAsync(customLogin => customLogin.Id == id);

        return customLogin;
    }
}
