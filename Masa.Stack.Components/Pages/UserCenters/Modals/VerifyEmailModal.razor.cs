namespace Masa.Stack.Components.UserCenters;

public partial class VerifyEmailModal : MasaComponentBase
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSuccess { get; set; }

    public VerifyMsgCodeModel VerifyMsgCode { get; set; } = new();

    public MForm FormRef { get; set; } = default!;

    private async Task<bool> SendCaptcha()
    {
        await AuthClient.UserService.SendMsgCodeAsync(new SendMsgCodeModel()
        {
            SendMsgCodeType = SendMsgCodeTypes.VerifiyPhoneNumber
        });
        return true;
    }

    private async Task HandleOnCancel()
    {
        FormRef.Reset();
        if (VisibleChanged.HasDelegate)
            await VisibleChanged.InvokeAsync(false);
        else Visible = false;
    }

    private async Task HandleOnOk()
    {
        if (FormRef.Validate())
        {
            var success = await AuthClient.UserService.VerifyMsgCodeAsync(VerifyMsgCode);
            if (success)
            {
                if (OnSuccess.HasDelegate)
                    await OnSuccess.InvokeAsync();

                await PopupService.EnqueueSnackbarAsync(T("Verify the phone number successfully"), AlertTypes.Success);
                await HandleOnCancel();
            }
            else
            {
                await PopupService.EnqueueSnackbarAsync(T("Verify the phone number failed"), AlertTypes.Error);
            }
        }
    }
}
