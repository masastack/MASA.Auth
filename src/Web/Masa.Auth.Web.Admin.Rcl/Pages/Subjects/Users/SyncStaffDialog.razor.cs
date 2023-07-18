// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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

    private long MaxFileSize { get; } = 2;

    private SyncStaffResultsDto? SyncStaffResults { get; set; }

    protected override void OnInitialized()
    {
        PageName = "StaffBlock";
        base.OnInitialized();
    }

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

        if (!visible)
        {
            File = null;
        }
    }

    private void OnFileChange(InputFileChangeEventArgs e)
    {
        File = e.File;
    }

    private void OnFileSelect()
    {
        File = null;
    }

    private void OnFileChange(IBrowserFile? file)
    {
        File = file;
    }

    //private async Task SyncAsync()
    //{
    //    if (File is not null)
    //    {
    //        Loading = true;
    //        using var content = new MultipartFormDataContent();
    //        var stream = File.OpenReadStream(MaxFileSize);
    //        var fileContent = new StreamContent(stream);
    //        fileContent.Headers.ContentType = new MediaTypeHeaderValue(File.ContentType);
    //        content.Add(fileContent, "\"files\"", File.Name);
    //        SyncStaffResults = await StaffService.SyncAsync(content);
    //        if (SyncStaffResults?.IsValid is false)
    //        {
    //            OpenSuccessMessage(T("Sync staff success"));
    //            await UpdateVisible(false);
    //            await OnSubmitSuccess.InvokeAsync();
    //        }
    //        else
    //        {
    //            OpenErrorMessage(T("Sync staff failed"));
    //        }
    //        Loading = false;
    //    }
    //}

    private async Task SyncAsync()
    {
        if (File is not null)
        {
            var fileContent = await ReadFile(File);
            if (FileEncoderHelper.GetTextFileEncodingType(fileContent) != Encoding.UTF8)
            {
                OpenWarningMessage(T("Please upload the UTF-8 encoded CSV file"));
                return;
            }
            var dto = new UploadFileDto(File.Name, fileContent, File.Size, "text/csv");
            SyncStaffResults = await StaffService.SyncAsync(dto);
            if (SyncStaffResults?.IsValid is false)
            {
                OpenSuccessMessage(T("Sync staff success"));
                await UpdateVisible(false);
                await OnSubmitSuccess.InvokeAsync();
            }
            else
            {
                OpenErrorMessage(T("Sync staff failed"));
            }
        }
        else OpenWarningMessage(T("Please upload CSV file"));
    }

    private async Task<byte[]> ReadFile(IBrowserFile file)
    {
        try
        {
            await using var memoryStream = new MemoryStream();
            using var readStream = file.OpenReadStream(1024 * 1024 * MaxFileSize);
            var bytesRead = 0;
            var totalRead = 0;
            var buffer = new byte[2048];

            while ((bytesRead = await readStream.ReadAsync(buffer)) != 0)
            {
                totalRead += bytesRead;

                await memoryStream.WriteAsync(buffer, 0, bytesRead);
            }
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("The requested file could not be read"))
                throw new Exception(T("When file update,need select file again"));
            else
                throw ex;
        }
    }
}

