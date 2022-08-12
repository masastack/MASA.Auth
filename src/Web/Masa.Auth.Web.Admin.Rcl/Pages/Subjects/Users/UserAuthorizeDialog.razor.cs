// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class UserAuthorizeDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid UserId { get; set; }

    public List<Guid> Teams { get; set; } = new();

    public UpdateUserAuthorizationDto Authorization { get; set; } = new();

    public UserDetailDto User { get; set; } = new();

    private UserService UserService => AuthCaller.UserService;

    public bool Preview { get; set; }

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

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            User = await UserService.GetDetailAsync(UserId);
            Authorization = new(User.Id, User.RoleIds, User.Permissions);
            Teams = new();
        }
    }

    private void PermissionsChanged(Dictionary<Guid, bool> permissiionMap)
    {
        Authorization.Permissions = permissiionMap.Select(kv => new SubjectPermissionRelationDto(kv.Key, kv.Value))
                                                   .ToList();
    }

    private async Task UpdateAuthorizationAsync()
    {
        Loading = true;
        await UserService.UpdateAuthorizationAsync(Authorization);
        OpenSuccessMessage(T("Successfully set user permissions"));
        await UpdateVisible(false);
        await OnSubmitSuccess.InvokeAsync();
        Loading = false;
    }

    public void TeamValueChanged(Guid value)
    {
        if (value != default) Teams = new() { value };
        else Teams = new();
    }
}

