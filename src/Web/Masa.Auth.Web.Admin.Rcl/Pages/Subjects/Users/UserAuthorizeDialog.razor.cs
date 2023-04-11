// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class UserAuthorizeDialog
{
    private bool _visible = false;
    private Guid _userId = Guid.Empty;
    private bool _preview = false;

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    public List<Guid> Teams { get; set; } = new();

    public UpdateUserAuthorizationDto Authorization { get; set; } = new();

    public UserDetailDto User { get; set; } = new();

    private UserService UserService => AuthCaller.UserService;

    public async Task ShowAsync(Guid userId)
    {
	_userId = userId;

	User = await UserService.GetDetailAsync(_userId);
	Authorization = new(User.Id, User.Roles.Select(role => role.Id), User.Permissions);
	Teams = new();

	_visible = true;
    }

    protected override void OnInitialized()
    {
        PageName = "UserBlock";
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
            _visible = visible;
        }
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

