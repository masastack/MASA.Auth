// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class CustomLoginThirdPartyIdpDto
{
    public Guid Id { get; set; }

    public int Sort { get; set; }

    public CustomLoginThirdPartyIdpDto()
    {
    }

    public CustomLoginThirdPartyIdpDto(Guid id, int sort)
    {
        Id = id;
        Sort = sort;
    }
}

