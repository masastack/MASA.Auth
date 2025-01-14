namespace Masa.Stack.Components.UserCenters;

public partial class UpdatePhoneNumberModal : MasaComponentBase
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback<string> OnSuccess { get; set; }

    public UpdateUserPhoneNumberModel UpdateUserPhoneNumber { get; set; } = new();

    public MForm FormRef { get; set; } = default!;


    private async Task<bool> SendCaptcha()
    {
        var field = FormRef.EditContext.Field(nameof(UpdateUserPhoneNumber.PhoneNumber));
        FormRef.EditContext.NotifyFieldChanged(field);
        var result = FormRef.EditContext.GetValidationMessages(field);
        if (!result.Any())
        {
            await AuthClient.UserService.SendMsgCodeAsync(new SendMsgCodeModel()
            {
                PhoneNumber = UpdateUserPhoneNumber.PhoneNumber,
                SendMsgCodeType = SendMsgCodeTypes.UpdatePhoneNumber
            });
        }
        return !result.Any();
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
            var success = await AuthClient.UserService.UpdatePhoneNumberAsync(UpdateUserPhoneNumber);
            if (success)
            {
                if (OnSuccess.HasDelegate)
                    await OnSuccess.InvokeAsync(UpdateUserPhoneNumber.PhoneNumber);

                await PopupService.EnqueueSnackbarAsync(T("Modify the phone number successfully"), AlertTypes.Success);
                await HandleOnCancel();
            }
            else
            {
                await PopupService.EnqueueSnackbarAsync(T("Modify the phone number failed"), AlertTypes.Error);
            }
        }
    }
}
