namespace Masa.Auth.Admin.Pages.Subjects.Users;

public partial class AddOrUpdateUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid UserId { get; set; } 

    private bool IsAdd => UserId == Guid.Empty;

    private UserItemResponse User { get; set; } = UserItemResponse.Default;

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
        if(Visible is true)
        {
            if (UserId == Guid.Empty) User = UserItemResponse.Default;
            else await GetUserDetailAsync();
        }
    }

    public async Task GetUserDetailAsync()
    {
        var response = await AuthClient.GetUserDetailAsync(UserId);
        if (response.Success)
        {
            User = response.Data ?? UserItemResponse.Default;
        }
        else OpenErrorMessage("Failed to query user data !");
    }

    public async Task AddOrUpdateUserAsync()
    {
        Lodding = true;
        if(IsAdd)
        {
            var reponse = await AuthClient.AddUserAsync(User);
            if (reponse.Success)
            {
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog("Failed to add user !");
        }
        else
        {
            var reponse = await AuthClient.EditUserAsync(User);
            if (reponse.Success)
            {
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog("Failed to edit user !");
        }
        Lodding = false;
    }
}

