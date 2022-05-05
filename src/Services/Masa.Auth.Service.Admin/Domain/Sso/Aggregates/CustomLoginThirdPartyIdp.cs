// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class CustomLoginThirdPartyIdp : Entity<int>
{
    public Guid ThirdPartyIdpId { get; private set; }

    public ThirdPartyIdp ThirdPartyIdp { get; private set; } = null!;

    public int CustomLoginId { get; private set; }

    public int Sort { get; private set; }

    public CustomLoginThirdPartyIdp(Guid thirdPartyIdpId, int sort)
    {
        ThirdPartyIdpId = thirdPartyIdpId;
        Sort = sort;
    }
}

