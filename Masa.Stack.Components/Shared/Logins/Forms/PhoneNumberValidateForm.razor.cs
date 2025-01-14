namespace Masa.Stack.Components.Forms;

public partial class PhoneNumberValidateForm : MasaComponentBase
{
    [CascadingParameter]
    public ForgetPasswordModal ForgetPasswordModal { get; set; } = null!;

    private bool _reseting = false;
    private ResetPasswordByPhoneModel _resetPasswordByPhoneModel = new();
    private MForm _form = null!;

    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(_resetPasswordByPhoneModel.PhoneNumber))
        {
            _resetPasswordByPhoneModel.PhoneNumber = MasaUser.PhoneNumber;
        }
        base.OnParametersSet();
    }

    internal void ResetFields()
    {
        _form.Reset();
    }

    private async Task HandleOnValidSubmit()
    {
        if (!_form.Validate())
        {
            return;
        }
        try
        {
            _reseting = true;
            await AuthClient.UserService.ResetPasswordByPhoneAsync(new ResetPasswordByPhoneModel
            {
                PhoneNumber = _resetPasswordByPhoneModel.PhoneNumber,
                Code = _resetPasswordByPhoneModel.Code,
                Password = _resetPasswordByPhoneModel.Password,
                ConfirmPassword = _resetPasswordByPhoneModel.ConfirmPassword
            });
            await PopupService.EnqueueSnackbarAsync(T("OperationSuccessfulMessage"), AlertTypes.Success);
            ForgetPasswordModal.HandleOnCancel();
            await ForgetPasswordModal.UpdatePasswordModal.HandleOnCancel();
        }
        catch (Exception e)
        {
            await PopupService.EnqueueSnackbarAsync(e.Message, AlertTypes.Error);
        }
        finally
        {
            _reseting = false;
        }
    }

    private async Task<bool> SendCaptcha()
    {
        var field = _form.EditContext.Field(nameof(_resetPasswordByPhoneModel.PhoneNumber));
        _form.EditContext.NotifyFieldChanged(field);
        var result = _form.EditContext.GetValidationMessages(field);
        if (!result.Any())
        {
            await AuthClient.UserService.SendMsgCodeAsync(new SendMsgCodeModel
            {
                SendMsgCodeType = SendMsgCodeTypes.ForgotPassword,
                PhoneNumber = _resetPasswordByPhoneModel.PhoneNumber
            });
        }
        return !result.Any();
    }

    class ResetPasswordByPhoneModelValidator : AbstractValidator<ResetPasswordByPhoneModel>
    {
        public ResetPasswordByPhoneModelValidator(I18n i18n)
        {
            RuleFor(c => c.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\s{0}$|^((\+86)|(86))?(1[3-9][0-9])\d{8}$").WithMessage(i18n.T("IncorrectFormat"))
                .WithName(i18n.T("PhoneNumber"));
            RuleFor(c => c.Code)
                .NotEmpty()
                .WithName(i18n.T("Captcha"));
            RuleFor(m => m.Password)
                .NotEmpty().WithMessage(i18n.T("PasswordRequired")).WithName(i18n.T("Password"))
                .Matches(@"^\s{0}$|^\S*(?=\S{6,})(?=\S*\d)(?=\S*[A-Za-z])\S*$");
            RuleFor(m => m.ConfirmPassword)
                .NotEmpty()
                .Equal(u => u.Password).WithMessage(i18n.T("FailToConfirmNewPassword"))
                .WithName(i18n.T("ConfirmNewPassword"));
        }
    }
}
