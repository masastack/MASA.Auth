// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class UploadImage : Upload
{
    [Parameter]
    public RenderFragment<List<string>>? ChildContent { get; set; }

    [Parameter]
    public string? DefaultImage { get; set; }

    [Parameter]
    public uint PreviewImageWith { get; set; }

    [Parameter]
    public uint PreviewImageHeight { get; set; }

    [Parameter]
    public string Icon { get; set; } = "./_content/Masa.Auth.Web.Admin.Rcl/img/upload/upload.svg";

    [Parameter]
    public bool Avatar { get; set; }

    [Parameter]
    public uint Size { get; set; }

    protected override void OnInitialized()
    {
        Accept = "image/*";
        OnInputFileChanged = "GetPreviewImageUrls";
        Class = "mr-4";
    }

    protected override async Task OnParametersSetAsync()
    {
        if (string.IsNullOrEmpty(DefaultImage) is false && string.IsNullOrEmpty(Value) && MultipleValue.Count == 0)
        {
            await SetValueAsync(new() { DefaultImage });
        }
    }

    protected override async Task OnInputFileChange(InputFileChangeEventArgs e)
    {
        await base.OnInputFileChange(e);
        if (OnInputFileChanged is not null && OnInputFileChanged.JsCallbackValue.Equals(default) is false)
        {
            var previewImageUrls = OnInputFileChanged.JsCallbackValue.Deserialize<List<string>>() ?? new();
            await SetValueAsync(previewImageUrls);
        }
    }

    string GetClass()
    {
        var css = Class;
        if (Avatar) css += " m-avatar";
        return css;
    }

    string GetStyle()
    {
        var style = "";
        if (Size > 0) style += $"height:{Size}px;width:{Size}px;";
        else if (PreviewImageWith > 0) style += $"width:{PreviewImageWith}px;";
        else if (PreviewImageHeight > 0) style += $"height:{PreviewImageHeight}px;";
        if (Avatar) style += $"border-radius: 50%;";
        return $"{style};{Style}";
    }
}

