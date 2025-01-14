namespace Masa.Stack.Components.Layouts;

public partial class UpdatePasswordModal
{
    private MForm _form = null!;
    private ForgetPasswordModal? _forgetPasswordModal;

    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    private UpdatePasswordModel Model { get; set; } = new();

    private bool HasOldPassword { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            HasOldPassword = await AuthClient.UserService.HasPasswordAsync();
        }
    }

    public async Task HandleOnCancel()
    {
        _form.Reset();
        await UpdateValueAsync(false);
    }

    async Task HandleOnOk()
    {
        var success = _form.Validate();
        if (success)
        {
            await AuthClient.UserService.UpdatePasswordAsync(new UpdateUserPasswordModel
            {
                NewPassword = Model.NewPassword,
                OldPassword = Model.OldPassword
            });
            await PopupService.EnqueueSnackbarAsync(@T("Update password success"), AlertTypes.Success);
            _form.Reset();
            await UpdateValueAsync(false);
        }
    }

    async Task UpdateValueAsync(bool value)
    {
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
        else Value = value;
    }
}
