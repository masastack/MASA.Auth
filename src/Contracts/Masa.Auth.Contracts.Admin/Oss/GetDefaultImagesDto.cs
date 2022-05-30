// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Oss;

public class GetDefaultImagesDto
{
    public GenderTypes Gender { get; set; }

    public string Url { get; set; }

    public GetDefaultImagesDto(GenderTypes gender, string url)
    {
        Gender = gender;
        Url = url;
    }
}

