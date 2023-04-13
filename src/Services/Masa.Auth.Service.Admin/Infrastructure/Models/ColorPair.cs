// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Models;

public class ColorPair
{
    public string FrontColor { get; set; }

    public string BackColor { get; set; }

    public ColorPair(string frontColor, string backColor)
    {
        FrontColor = frontColor;
        BackColor = backColor;
    }
}
