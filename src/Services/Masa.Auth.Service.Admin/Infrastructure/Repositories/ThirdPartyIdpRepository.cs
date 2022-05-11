// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Repositories;

public class ThirdPartyIdpRepository : Repository<AuthDbContext, ThirdPartyIdp>, IThirdPartyIdpRepository
{
    public ThirdPartyIdpRepository(AuthDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
