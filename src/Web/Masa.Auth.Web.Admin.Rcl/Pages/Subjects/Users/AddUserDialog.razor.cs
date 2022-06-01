// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private int Step { get; set; } = 1;

    private bool Fill { get; set; }

    private MForm? Form { get; set; }

    private AddUserDto User { get; set; } = new();

    private UserService UserService => AuthCaller.UserService;

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
            Visible = false;
        }
        if (Form is not null)
        {
            await Form.ResetValidationAsync();
        }
    }

    protected override void OnParametersSet()
    {
        if (Visible is true)
        {
            Step = 1;
            Fill = false;
            User = new();
            ChangeAvayar();
        }
    }

    private void NextStep(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Step = 3;
        }
    }

    private void PermissionsChanged(Dictionary<Guid, bool> permissiionMap)
    {
        User.Permissions = permissiionMap.Select(kv => new UserPermissionDto(kv.Key, kv.Value)).ToList();
    }

    public async Task AddUserAsync()
    {
        Loading = true;
        if (DefaultUploadImageRef is not null) await DefaultUploadImageRef.UploadAsync();

        await UserService.AddAsync(User);
        OpenSuccessMessage(T("Add user data success"));
        await UpdateVisible(false);
        await OnSubmitSuccess.InvokeAsync();
        Loading = false;
    }

    private void ChangeAvayar()
    {
        Random random = new Random();
        var images = DefaultImages.Where(image => image.Gender == User.Gender).ToList();
        if (images.Count > 0)
        {
            var avatar = images[random.Next(0, images.Count)].Url;
            if (avatar == User.Avatar && images.Count > 1) ChangeAvayar();
            else User.Avatar = avatar;
        }
    }
}

