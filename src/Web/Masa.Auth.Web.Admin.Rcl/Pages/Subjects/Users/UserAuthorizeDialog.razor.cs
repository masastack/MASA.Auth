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

    public List<Guid> TeamRoles { get; set; } = new();

    public UpdateUserAuthorizationDto Authorization { get; set; } = new();

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
            var user = await UserService.GetDetailAsync(UserId);
            Authorization = new(user.Id, user.RoleIds, user.Permissions);
        }
    }

    private void PermissionsChanged(Dictionary<Guid, bool> permissiionMap)
    {
        Authorization.Permissions = permissiionMap.Select(kv => new UserPermissionDto(kv.Key, kv.Value))
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
}

