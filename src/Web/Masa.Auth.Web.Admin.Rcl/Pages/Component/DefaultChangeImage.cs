// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Components.Rendering;

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

    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        base.BuildRenderTree(__builder);
        __builder.OpenRegion(18);
        __builder.OpenElement(0, "div");
        __builder.AddAttribute(1, "class", "mt-3 hover-pointer font-16");

        __builder.OpenComponent<MIcon>(2);
        __builder.AddAttribute(3, "Size", (StringNumber)18);
        __builder.AddAttribute(4, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder __builder2) {
            __builder2.AddContent(5, "mdi-autorenew");
        });//Todo:need change to IconConstants.Refresh
        __builder.CloseComponent();

        __builder.OpenElement(6, "span");
        __builder.AddAttribute(7, "class", "ml-3");
        __builder.AddAttribute(8, "onclick", EventCallback.Factory.Create(this, ChangeAvayarAsync));
        __builder.AddContent(9, T("Another"));
        __builder.CloseElement();

        __builder.CloseElement();
        __builder.CloseRegion();
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
                await SetValueAsync(new() { avatar });//need change to SetValueAsync(avatar)
            }
        }
    }
}
