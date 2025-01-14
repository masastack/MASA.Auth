// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public partial class SUploadImage : SUpload
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
	public string Icon { get; set; } = "./_content/Masa.Stack.Components/img/upload/upload.svg";

	[Parameter]
	public bool Avatar { get; set; }

	[Parameter]
	public uint Size { get; set; }

	[Parameter]
	public bool IsOverlay { get; set; }

	[Parameter]
	public int OverlayOpenDelay { get; set; }

	[Parameter]
	public int OverlayCloseDelay { get; set; }

	[Parameter]
	public string OverlayTips { get; set; } = string.Empty;

	public override async Task SetParametersAsync(ParameterView parameters)
	{
		Accept = "image/*";
		OnInputFileChanged = "GetPreviewImageUrls";
		MaximumFileSize = 1024 * 1024 * 2;
		await base.SetParametersAsync(parameters);
	}

	protected override async Task OnParametersSetAsync()
	{
		if (string.IsNullOrEmpty(DefaultImage) is false && string.IsNullOrEmpty(Value) && MultipleValue.Count == 0)
		{
			await SetValueAsync(DefaultImage);
		}
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			var module = await JS.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Stack.Components/Shared/IntegrationComponents/Upload/SUploadImage.razor.js");
			await module.InvokeVoidAsync("calculate");
		}
		await base.OnAfterRenderAsync(firstRender);
	}

	string GetClass()
	{
		var css = Class;
		css += " mx-auto hover-misc-transition";
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

	List<string> getSrcList() => MultipleValue.Count == 0 ? new List<string> { Icon } : MultipleValue;
}
