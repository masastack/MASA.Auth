// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Utils;

/// <summary>
/// 团队头像生成：取名称首字符，按配色对画一张方形 PNG。
/// 用 SkiaSharp（MIT）而非 SixLabors.ImageSharp：后者自 3.2.0 起在 Release 编译期强制校验商业许可，
/// 会给 CI/镜像构建引入一个与开源项目无关的密钥注入点。这里只用到"填底色 + 居中一个字形 + 存 PNG"，
/// Skia 完全覆盖，且显式加载字体文件，不依赖系统字体栈（aspnet 运行时镜像里没有 fontconfig）。
/// </summary>
public static class AvatarImageGenerator
{
    const string FONT_RELATIVE_PATH = "Assets/Fonts/SourceHanSansCN-Normal.ttf";

    //字体约 10MB，解析一次后进程内复用；加载失败（容器漏拷资源等）时降级为纯色块，
    //不能让头像生成把创建团队的主流程带崩——调用处本来也把整段包在 try/catch 里。
    static readonly Lazy<SKTypeface?> _typeface = new(LoadTypeface, LazyThreadSafetyMode.ExecutionAndPublication);

    public static MemoryStream GeneratePortrait(char show, string textColor, string backgroundColor, int size)
    {
        var ms = new MemoryStream();
        Render(show, textColor, backgroundColor, size, ms);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }

    public static void GeneratePortrait(char show, string textColor, string backgroundColor, int size, string path)
    {
        using var file = File.Create(path);
        Render(show, textColor, backgroundColor, size, file);
    }

    static void Render(char show, string textColor, string backgroundColor, int size, Stream output)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        var info = new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info)
            ?? throw new InvalidOperationException("Unable to create a Skia surface for avatar generation.");
        var canvas = surface.Canvas;
        canvas.Clear(SKColor.Parse(backgroundColor));

        var typeface = _typeface.Value;
        if (typeface is not null && show != default && !char.IsWhiteSpace(show))
        {
            using var paint = new SKPaint { Color = SKColor.Parse(textColor), IsAntialias = true };
            using var font = new SKFont(typeface, size * 0.6f) { Edging = SKFontEdging.Antialias };

            //按字形实际上下沿居中，而不是按字号估算：中文与拉丁字母的字面高度差异明显。
            var metrics = font.Metrics;
            var baseline = size / 2f - (metrics.Ascent + metrics.Descent) / 2f;
            canvas.DrawText(show.ToString(), size / 2f, baseline, SKTextAlign.Center, font, paint);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        data.SaveTo(output);
    }

    /// <summary>
    /// 优先按程序目录找资源（发布/测试场景），再退回工作目录（dotnet run 场景）。
    /// </summary>
    static SKTypeface? LoadTypeface()
    {
        foreach (var path in new[] { Path.Combine(AppContext.BaseDirectory, FONT_RELATIVE_PATH), FONT_RELATIVE_PATH })
        {
            try
            {
                if (File.Exists(path))
                {
                    return SKTypeface.FromFile(path);
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
        return null;
    }
}
