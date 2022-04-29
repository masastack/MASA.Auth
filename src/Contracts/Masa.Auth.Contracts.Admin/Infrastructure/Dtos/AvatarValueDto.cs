// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Dtos;

public class AvatarValueDto
{
    public string Url { get; set; } = "";

    public string Name { get; set; } = "";

    public string Color { get; set; } = "";

    public AvatarValueDto()
    {

    }

    public AvatarValueDto(string url, string name, string color)
    {
        Url = url;
        Name = name;
        Color = color;
    }
}
