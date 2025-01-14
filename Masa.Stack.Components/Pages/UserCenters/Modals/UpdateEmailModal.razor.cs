namespace Masa.Stack.Components.UserCenters;

public partial class UpdateEmailModal : MasaComponentBase
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback<string> OnSuccess { get; set; }

    public UpdateUserEmailModel UpdateUserEmail { get; set; } = new();

    public MForm FormRef { get; set; } = default!;


    private async Task<bool> SendCaptcha()
    {
        var field = FormRef.EditContext.Field(nameof(UpdateUserEmail.Email));
        FormRef.EditContext.NotifyFieldChanged(field);
        var result = FormRef.EditContext.GetValidationMessages(field);
        if (!result.Any())
        {
            await AuthClient.UserService.SendEmailAsync(new SendEmailModel()
            {
                Email = UpdateUserEmail.Email,
                SendEmailType = SendEmailTypes.UpdateEmail
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

    private void HandleOnOk()
    {
    }
}
