namespace Masa.Auth.Web.Admin.Rcl.Pages.Permissions.Roles;

public partial class AddOrEditRoleDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid RoleId { get; set; }

    private bool IsAdd => RoleId == Guid.Empty;

    private RoleDetailResponse Role { get; set; } = RoleDetailResponse.Default;

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
        if (Visible is true)
        {
            if (IsAdd) Role = RoleDetailResponse.Default;
            else await GetRoleDetailAsync();
        }
    }

    public async Task GetRoleDetailAsync()
    {
        var response = await AuthClient.GetRoleDetailAsync(RoleId);
        if (response.Success)
        {
            Role = response.Data;
        }
        else OpenErrorMessage(T("Failed to query roleDetail data:") + response.Message);
    }

    public async Task AddOrEditRoleAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            var response = await AuthClient.AddRoleAsync(Role);
            if (response.Success)
            {
                OpenSuccessMessage(T("Add role data success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to add role:") + response.Message);
        }
        else
        {
            var response = await AuthClient.EditRoleAsync(Role);
            if (response.Success)
            {
                OpenSuccessMessage(T("Edit role data success"));
                await OnSubmitSuccess.InvokeAsync();
                await UpdateVisible(false);
            }
            else OpenErrorDialog(T("Failed to edit role:") + response.Message);
        }
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

