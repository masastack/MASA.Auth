// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class ThirdPartyIdpDetailDto : ThirdPartyIdpDto
{
    public bool MapAll { get; set; }

    public Dictionary<string, string> JsonKeyMap { get; set; } = new();

    public ThirdPartyIdpDetailDto()
    {          
    }
}

