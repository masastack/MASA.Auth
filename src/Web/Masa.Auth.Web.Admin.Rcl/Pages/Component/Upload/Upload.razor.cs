// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class Upload
{
    [Inject]
    public IJSRuntime? Js { get; set; }

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public bool Multiple { get; set; }

    [Parameter]
    public string Accept { get; set; } = "";

    [Parameter]
    public string Capture { get; set; } = "";

    [Parameter]
    public int MaximumFileCount { get; set; } = 10;

    [Parameter]
    public string? Value
    {
        get => MultipleValue.FirstOrDefault();
        set
        {
            if (Multiple is false)
            {
                MultipleValue.Clear();
                if (string.IsNullOrEmpty(value) is false)
                {
                    MultipleValue.Add(value);
                }
            }
        }
    }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    [Parameter]
    public List<string> MultipleValue { get; set; } = new();

    [Parameter]
    public EventCallback<List<string>> MultipleValueChanged { get; set; }

    [Parameter]
    public FileChangeCallBack? OnInputFileChanged { get; set; }

    [Parameter]
    public FileUploadCallBack? OnInputFileUpload { get; set; }

    [Parameter]
    public bool WhenFileChangeUpload { get; set; }

    public InputFile? InputFileRef { get; set; }

    public IReadOnlyList<IBrowserFile> Files { get; set; } = new List<IBrowserFile>();

    IJSObjectReference? UploadJs { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            UploadJs = await Js!.InvokeAsync<IJSObjectReference>("import", "./_content/Masa.Auth.Web.Admin.Rcl/js/upload.js");
        }
    }

    protected virtual async Task OnInputFileChange(InputFileChangeEventArgs e)
    {
        Files = e.GetMultipleFiles(MaximumFileCount);
        if (OnInputFileChanged is null) return;
        if (OnInputFileChanged.IsJsCallback)
        {
            OnInputFileChanged.JsCallbackValue = await UploadJs!.InvokeAsync<JsonElement>("InputFileChanged", InputFileRef?.Element, OnInputFileChanged.JsCallback);
        }
        else if (OnInputFileChanged.IsDelegateCallback)
        {
            await OnInputFileChanged.DelegateCallback(Files);
        }
        else if (OnInputFileChanged.IsEventCallback)
        {
            await OnInputFileChanged.EventCallback.InvokeAsync(Files);
        }

        if (WhenFileChangeUpload) await UploadAsync();
    }

    public virtual async Task UploadAsync()
    {
        var values = new List<string>();
        if (OnInputFileUpload is null) return;
        if (OnInputFileUpload.IsJsCallback)
        {
            var multipleValue = await UploadJs!.InvokeAsync<List<string>>("InputFileUpload", InputFileRef?.Element, OnInputFileUpload.JsCallback, OnInputFileUpload.JsCallBackParamter);
            values.AddRange(multipleValue);
        }
        else if (OnInputFileUpload.IsDelegateCallback)
        {
            values.AddRange(await OnInputFileUpload.DelegateCallback(Files));
        }
        else if (OnInputFileUpload.IsEventCallback)
        {
            await OnInputFileUpload.EventCallback.InvokeAsync((Files, value => values = value));
        }

        await SetValueAsync(values);
    }

    protected async Task SetValueAsync(List<string> values)
    {
        if (Multiple)
        {
            MultipleValue = values;
            if (MultipleValueChanged.HasDelegate) await MultipleValueChanged.InvokeAsync(MultipleValue);
        }
        else
        {
            Value = values.FirstOrDefault();
            if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(Value);
        }
    }
}

