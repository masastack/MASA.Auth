// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddStaffDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private MForm? Form { get; set; }

    private int Step { get; set; } = 1;

    private AddStaffDto Staff { get; set; } = new();

    private StaffService StaffService => AuthCaller.StaffService;

    private OssService OssService => AuthCaller.OssService;

    private List<GetDefaultImagesDto> DefaultImages { get; set; } = new();

    private DefaultUploadImage? DefaultUploadImageRef { get; set; }

    protected override async Task OnInitializedAsync()
    {
        DefaultImages = await OssService.GetDefaultImagesAsync();
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
        if (Form is not null)
        {
            await Form.ResetValidationAsync();
        }
    }

    protected override void OnParametersSet()
    {
        if (Visible)
        {
            Staff = new();
            Step = 1;
        }
    }

    public async Task AddStaffAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            if (DefaultUploadImageRef is not null) await DefaultUploadImageRef.UploadAsync();
            await StaffService.AddAsync(Staff);
            OpenSuccessMessage(T("Add staff success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }

    private void ChangeAvayar()
    {
        Random random = new Random();
        var images = DefaultImages.Where(image => image.Gender == Staff.User.Gender).ToList();
        if (images.Count > 0)
        {
            var avatar = images[random.Next(0, images.Count)].Url;
            if (avatar == Staff.User.Avatar && images.Count > 1) ChangeAvayar();
            else Staff.User.Avatar = avatar;
        }
    }
}

