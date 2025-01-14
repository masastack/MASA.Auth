namespace Masa.Stack.Components.UserCenters;

public partial class StaffInfo : MasaComponentBase
{
    private int _windowValue = 0;

    public StaffDetailModel StaffDetail { get; set; } = new();

    public UpdateStaffBasicInfoModel UpdateStaff { get; set; } = new();

    public UpdateStaffAvatarModel UpdateStaffAvatar { get; set; } = new();

    private Dictionary<string, object?>? Items { get; set; }

    private Dictionary<string, object?>? PreviewItems { get; set; }

    private bool UpdateUserNameVisible { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetCurrentStaffAsync();
            StateHasChanged();
        }
    }

    private async Task GetCurrentStaffAsync()
    {
        StaffDetail = await AuthClient.UserService.GetCurrentStaffAsync() ?? throw new UserFriendlyException("This staff does not exist");
        if (StaffDetail.Enabled is false)
        {
            await PopupService.EnqueueSnackbarAsync(T("The employee account has been frozen, please contact the administrator!"), AlertTypes.Error);
            NavigationManager.NavigateTo("/user-center");
        }
        UpdateStaff = StaffDetail.Adapt<UpdateStaffBasicInfoModel>();
        UpdateStaffAvatar = new(default, StaffDetail.Avatar);
        Items = new Dictionary<string, object?>()
        {
            ["Position"] = ("mdi-briefcase", StaffDetail.Position),
            ["JobNumber"] = ("mdi-office-building", StaffDetail.JobNumber),
            ["StaffType"] = ("mdi-office-building", StaffDetail.StaffType.ToString()),
            ["Address"] = ("mdi-map-marker", StaffDetail.Address.Address),
            ["Department"] = ("mdi-file-tree", StaffDetail.Department),
            ["CreationTime"] = ("mdi-clock-outline", StaffDetail.CreationTime.ToString("yyyy-MM-dd")),
        };

        PreviewItems = new Dictionary<string, object?>()
        {
            ["PhoneNumber"] = ("mdi-phone", StaffDetail.PhoneNumber),
            ["Email"] = ("mdi-email", StaffDetail.Email)
        };
    }

    private async Task UpdateBasicInfoAsync()
    {
        await AuthClient.UserService.UpdateStaffBasicInfoAsync(UpdateStaff);
        await GetCurrentStaffAsync();
        _windowValue = default;
    }

    private async Task UpdateAvatarAsync(string avatar)
    {
        UpdateStaffAvatar.Avatar = avatar;
        await AuthClient.UserService.UpdateStaffAvatarAsync(UpdateStaffAvatar);
        await GetCurrentStaffAsync();
        _windowValue = default;
    }

    private void Cancel()
    {
        UpdateStaff = StaffDetail.Adapt<UpdateStaffBasicInfoModel>();
        _windowValue = default;
    }

    private void NavigateToUser()
    {
        NavigationManager.NavigateTo("/user-center");
    }

    public Task EnableUpdateName(MouseEventArgs _)
    {
        UpdateUserNameVisible = true;
        return Task.CompletedTask;
    }

    private async Task UpdateNameCancelAsync()
    {
        await GetCurrentStaffAsync();
        UpdateUserNameVisible = false;
    }

    private async Task UpdateNameConfirmAsync()
    {
        await AuthClient.UserService.UpdateStaffBasicInfoAsync(UpdateStaff);
        await GetCurrentStaffAsync();
        UpdateUserNameVisible = false;
    }
}
