// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Service.Admin.Infrastructure.Utils;
using SkiaSharp;
using Xunit;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// 头像生成已从 SixLabors.ImageSharp 换成 SkiaSharp（MIT，避开 3.2.0 起的 Release 许可校验）。
/// 这里验证替换后确实产出了可解码的 PNG，且配色生效——头像失败在业务上会被 try/catch 静默降级成默认图，
/// 只有测试能兜住"生成的其实是空图/黑图"这类回归。
/// </summary>
public class AvatarImageGeneratorTests
{
    [Theory]
    [InlineData('A', "#4318FF", "#F4F1FF")]
    [InlineData('中', "#05CD99", "#E6FAF5")]
    public void GeneratePortrait_ProducesDecodablePng(char initial, string front, string back)
    {
        using var stream = AvatarImageGenerator.GeneratePortrait(initial, front, back, 200);

        Assert.True(stream.CanRead);
        Assert.Equal(0, stream.Position);

        using var bitmap = SKBitmap.Decode(stream);
        Assert.NotNull(bitmap);
        Assert.Equal(200, bitmap.Width);
        Assert.Equal(200, bitmap.Height);
    }

    [Fact]
    public void GeneratePortrait_PaintsBackgroundColorAtCorner()
    {
        using var stream = AvatarImageGenerator.GeneratePortrait('A', "#4318FF", "#F4F1FF", 64);
        using var bitmap = SKBitmap.Decode(stream);

        //角上不会有字形，应当正好是背景色
        var corner = bitmap.GetPixel(0, 0);
        Assert.Equal(SKColor.Parse("#F4F1FF"), corner);
    }

    [Fact]
    public void GeneratePortrait_DrawsForegroundSomewhereInside()
    {
        using var stream = AvatarImageGenerator.GeneratePortrait('A', "#4318FF", "#F4F1FF", 64);
        using var bitmap = SKBitmap.Decode(stream);

        var background = SKColor.Parse("#F4F1FF");
        var hasForeground = false;
        for (var y = 0; y < bitmap.Height && !hasForeground; y++)
        {
            for (var x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y) != background)
                {
                    hasForeground = true;
                    break;
                }
            }
        }
        Assert.True(hasForeground, "头像上没有任何前景像素，说明字形绘制失败（很可能是字体没能加载）");
    }

    [Fact]
    public void GeneratePortrait_BlankInitialStillProducesValidImage()
    {
        //空名称时调用方会传 char 默认值；此时应退化为纯色块而不是抛异常
        using var stream = AvatarImageGenerator.GeneratePortrait(default, "#4318FF", "#F4F1FF", 32);
        using var bitmap = SKBitmap.Decode(stream);

        Assert.Equal(32, bitmap.Width);
        Assert.Equal(SKColor.Parse("#F4F1FF"), bitmap.GetPixel(0, 0));
    }

    [Fact]
    public void GeneratePortrait_RejectsNonPositiveSize()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => AvatarImageGenerator.GeneratePortrait('A', "#4318FF", "#F4F1FF", 0));
    }
}
