﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Magicodes.IE.Core;
using System.IO;

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public static class ImageSharper
{
    static Image<Rgba32> Generate(char show, Color textColor, Color backgroundColor, int size)
    {
        var image = new Image<Rgba32>(size, size);
        image.Mutate(x => x.BackgroundColor(backgroundColor));
        var textOptions = new RichTextOptions(new Font(GetFontFamily(), (int)(size * 0.6)))
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Origin = new Vector2(size / 2, size / 2)
        };
        image.Mutate(x => x.DrawText(textOptions, show.ToString(), textColor));
        return image;
    }

    public static MemoryStream GeneratePortrait(char show, Color textColor, Color backgroundColor, int size)
    {
        var ms = new MemoryStream();
        using var image = Generate(show, textColor, backgroundColor, size);       
        image.Save(ms, SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }

    public static void GeneratePortrait(char show, Color textColor, Color backgroundColor, int size, string path)
    {
        using var image = Generate(show, textColor, backgroundColor, size);
        image.SaveAsPng(path);
    }

    private static FontFamily GetFontFamily()
    {
        var fonts = new FontCollection();
        if (File.Exists("./Assets/Fonts/SourceHanSansCN-Normal.ttf"))
        {
            return fonts.Add("./Assets/Fonts/SourceHanSansCN-Normal.ttf");
        }
        else
        {
            return SystemFonts.Families.FirstOrDefault();
        }
    }
}
