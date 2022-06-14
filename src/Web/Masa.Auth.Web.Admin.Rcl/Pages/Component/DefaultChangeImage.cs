// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultChangeImage : DefaultUploadImage
{
    [Parameter]
    public GenderTypes Gender { get; set; }

    private List<GetDefaultImagesDto> DefaultImages { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        DefaultImages = await OssService.GetDefaultImagesAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (string.IsNullOrEmpty(Value)) await ChangeAvayarAsync();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.OpenRegion(18);
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "mt-3 hover-pointer body primary--text");
        builder.AddAttribute(2, "onclick", EventCallback.Factory.Create(this, ChangeAvayarAsync));

        builder.OpenComponent<MIcon>(3);
        builder.AddAttribute(4, "Size", (StringNumber)18);
        builder.AddAttribute(5, "Class", "primary--text");
        builder.AddAttribute(6, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder2) {
            builder2.AddContent(7, IconConstants.Refresh);
        });
        builder.CloseComponent();

        builder.OpenElement(8, "span");
        builder.AddAttribute(9, "class", "ml-3 body");
        builder.AddContent(10, T("Another"));
        builder.CloseElement();

        builder.CloseElement();
        builder.CloseRegion();
    }

    private async Task ChangeAvayarAsync()
    {
        Random random = new Random();
        var images = DefaultImages.Where(image => image.Gender == Gender).ToList();
        if (images.Count > 0)
        {
            var avatar = images[random.Next(0, images.Count)].Url;
            if (avatar == Value && images.Count > 1) await ChangeAvayarAsync();
            else
            {
                await SetValueAsync(avatar);
            }
        }
    }
}
