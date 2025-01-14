namespace Masa.Stack.Components.Forms;

public partial class EmailValidateForm : MasaComponentBase
{

    [CascadingParameter]
    public ForgetPasswordModal ForgetPasswordModal { get; set; } = null!;

    private bool _valid;
    private MForm _form = null!;

    public string? EmailAddress { get; set; }

    protected override void OnParametersSet()
    {
        if (string.IsNullOrEmpty(EmailAddress))
        {
            EmailAddress = MasaUser.Email;
        }
        base.OnParametersSet();
    }

    internal void ResetFields()
    {
        _valid = false;

        _form.Reset();
    }

    private void HandleOnValidSubmit()
    {
        // TODO: send a email
        // TODO: 需要一个点击邮箱链接直达更改密码的页面
    }

    private void ValidateEmail(string? val)
    {
        // TODO: 验证输入的email不是旧的email

        // TODO: 正则表达式是否不太严谨，或者有其他需求，需要Auth同学确认
        _valid = IsValid(val);
    }

    private static bool IsValid(string? val)
    {
        if (val is null)
        {
            return false;
        }

        var match = Regex.Match(val, @"^[a-zA-Z0-9.!#$%&’*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$");
        return match.Success;
    }

    class EmailValidator : AbstractValidator<EmailValidateForm>
    {
        public EmailValidator(I18n i18N)
        {
            RuleFor(c => c.EmailAddress)
                .NotEmpty()
                .Must(IsValid).WithMessage(i18N.T("IncorrectFormat"))
                .WithName(i18N.T("EmailAddress"));
        }
    }
}
