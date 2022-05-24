// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Utils;

public static class ImageSharper
{
    public static MemoryStream GeneratePortrait(char show, Color textColor, Color backgroundColor, int size)
    {
        MemoryStream ms = new MemoryStream();
        using var image = new Image<Rgba32>(size, size);
            image.Mutate(x => x.BackgroundColor(backgroundColor));
            var textOptions = new TextOptions(new Font(SystemFonts.Get("STKAITI"), size))
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Origin = new Vector2(size / 2, size / 2)
            };
            image.Mutate(x => x.DrawText(textOptions, show.ToString(), textColor));
            image.SaveAsPng(ms);
        }
        return ms;
    }

    public static void GeneratePortrait(char show, Color textColor, Color backgroundColor, int size, string path)
    {
        using var image = new Image<Rgba32>(size, size);
            image.Mutate(x => x.BackgroundColor(backgroundColor));
            var textOptions = new TextOptions(new Font(SystemFonts.Get("STKAITI"), size))
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Origin = new Vector2(size / 2, size / 2)
            };
            image.Mutate(x => x.DrawText(textOptions, show.ToString(), textColor));
            image.SaveAsPng(path);
        }
    }
}
