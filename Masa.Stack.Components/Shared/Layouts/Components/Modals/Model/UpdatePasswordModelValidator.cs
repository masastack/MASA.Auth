namespace Masa.Stack.Components.Layouts;

public class UpdatePasswordModelValidator : AbstractValidator<UpdatePasswordModel>
{
    public UpdatePasswordModelValidator(I18n i18N)
    {
        RuleFor(m => m.NewPassword)
            .NotEmpty()
            .Matches(@"^\S*(?=\S{6,})(?=\S*\d)(?=\S*[A-Za-z])\S*$")
            .WithMessage(i18N.T("PasswordFormatVerificationPrompt"))
            .WithName(i18N.T("NewPassword"))
            .NotEqual(u => u.OldPassword)
            .WithMessage(i18N.T("EqualOldAndNewPassword"))
            .WithName(i18N.T("NewPassword"));

        RuleFor(m => m.ConfirmNewPassword)
            .NotEmpty()
            .Equal(u => u.NewPassword).WithMessage(i18N.T("FailToConfirmNewPassword"))
            .WithName(i18N.T("ConfirmNewPassword"));
    }
}
