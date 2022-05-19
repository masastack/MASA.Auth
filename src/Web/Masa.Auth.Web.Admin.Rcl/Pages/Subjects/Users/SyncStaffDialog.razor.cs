// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Net.Http;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class SyncStaffDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private StaffService StaffService => AuthCaller.StaffService;

    private IBrowserFile? File { get; set; }

    private long MaxFileSize { get; } = 1024 * 1024 * 15;

    private SyncStaffResultsDto? SyncStaffResults { get; set; }

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    private void OnFileChange(IBrowserFile file)
    {
        File = file;
    }

    private async Task SyncAsync()
    {
        if (File is not null)
        {
            Loading = true;
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(File.OpenReadStream(MaxFileSize));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(File.ContentType);
            content.Add(fileContent, "\"files\"", File.Name);
            SyncStaffResults = await StaffService.SyncAsync(content);
            if(SyncStaffResults?.IsValid is false)
            {
                OpenSuccessMessage(T("Sync staff success"));
                await UpdateVisible(false);
                await OnSubmitSuccess.InvokeAsync();
            }
            else
            {
                OpenErrorMessage(T("Sync staff failed"));                
            }
            Loading = false;
        }
    }
}

